using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexoMovil.Api.Response;
using NexoMovil.Core.DTOs;
using NexoMovil.Core.Enums;
using NexoMovil.Services.Interfaces;
using NexoMovil.Services.Validators.User;

namespace NexoMovil.Api.Controllers
{
    /// <summary>
    /// CU-07: Gestionar Usuarios. RN-06: únicamente el Administrador
    /// puede crear, actualizar, desactivar y reactivar usuarios.
    /// </summary>
    [Authorize(Roles = nameof(RoleType.Admin))]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly CrearUsuarioDtoValidator _crearValidator;
        private readonly ActualizarUsuarioDtoValidator _actualizarValidator;

        public UserController(
            IUserService userService,
            IMapper mapper,
            CrearUsuarioDtoValidator crearValidator,
            ActualizarUsuarioDtoValidator actualizarValidator)
        {
            _userService = userService;
            _mapper = mapper;
            _crearValidator = crearValidator;
            _actualizarValidator = actualizarValidator;
        }

        // GET: api/user
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<UserDto>>(users));
        }

        // GET: api/user/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado." });

            return Ok(new ApiResponse<UserDto>(user));
        }

        // POST: api/user
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            var validation = await _crearValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                throw new ValidationException(validation.Errors);

            var user = await _userService.CreateUserAsync(dto);
            var createdDto = _mapper.Map<UserDto>(user);

            return CreatedAtAction(
                nameof(GetById),
                new { id = user.Id },
                new ApiResponse<UserDto>(createdDto));
        }

        // PUT: api/user/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
        {
            var validation = await _actualizarValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                throw new ValidationException(validation.Errors);

            var user = await _userService.UpdateUserAsync(id, dto);
            var response = new ApiResponse<UserDto>(_mapper.Map<UserDto>(user));

            return Ok(response);
        }

        // PATCH: api/user/1/activar
        [HttpPatch("{id:int}/activar")]
        public async Task<IActionResult> Activate(int id)
        {
            await _userService.ActivateUserAsync(id);

            var response = new ApiResponse<string>(
                $"Usuario con ID {id} reactivado correctamente. Ya puede iniciar sesión.");
            return Ok(response);
        }

        // DELETE: api/user/1 (baja lógica)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _userService.DeactivateUserAsync(id);

            var response = new ApiResponse<string>(
                $"Usuario con ID {id} desactivado correctamente. Ya no podrá iniciar sesión.");
            return Ok(response);
        }
    }
}
