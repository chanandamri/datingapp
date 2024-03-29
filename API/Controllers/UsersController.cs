﻿using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helper;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]

    public class UsersController : BaseAPIController
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IKululuService _kuluService;
        private readonly IPhotoService _photoService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper,
            ILogger<UsersController> logger, IKululuService kuluService
            , IPhotoService photoService)
        {
            _kuluService = kuluService;
            _photoService = photoService;
            _logger = logger;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
        {
            var currentUser = await _userRepository.GetUserByuserNameAsync(User.GetUsername());
            userParams.CurrentUsername = currentUser.UserName;

            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = currentUser.Gender == "female" ? "male" : "female";
            }

            _logger.LogInformation("David is the king");
            _kuluService.DoTheChacha();

            var users = await _userRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));
            return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var user = await _userRepository.GetUserByuserNameAsync(User.GetUsername());
            if (user == null) return NotFound();

            _mapper.Map(memberUpdateDto, user);

            if (await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to update user");
        }
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserByuserNameAsync(User.GetUsername());
            if (user == null) return NotFound();
            var result = await _photoService.AddPhotoAsync(file);
            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if (user.Photos.Count == 0) photo.IsMain = true;
            user.Photos.Add(photo);
            if (await _userRepository.SaveAllAsync())
            {
                return CreatedAtAction(nameof(GetUser),
                    new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));
            }
            return BadRequest("Problem Adding photo");
        }
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByuserNameAsync(User.GetUsername());
            if (user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo != null) NotFound();
            if (photo.IsMain) BadRequest("This is already your main photo");
            var CurrentMain = user.Photos.FirstOrDefault(x => x.IsMain == true);
            if (CurrentMain != null) CurrentMain.IsMain = false;
            photo.IsMain = true;

            if (await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Problem setting main photo");


        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _userRepository.GetUserByuserNameAsync(User.GetUsername());
            if (user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("Can't delete main photo");

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }
            user.Photos.Remove(photo);
            if (await _userRepository.SaveAllAsync()) return Ok();
            return BadRequest("Problem deleting photo");



        }


    }
}
