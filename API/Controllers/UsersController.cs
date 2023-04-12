using API.DTOs;
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
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper,
            ILogger<UsersController> logger, IKululuService kuluService)
        {
            _kuluService = kuluService;
            _logger = logger;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            _logger.LogInformation("David is the king");
            _kuluService.DoTheChacha();

            var users = await _userRepository.GetMembersAsnc();
            return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);
        }
    }
}
