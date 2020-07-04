using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Commander.Data;
using Commander.Dtos;
using Commander.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Commander.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly CommanderContext _db;

        public CommandsController(IMapper mapper, CommanderContext db)
        {
            _mapper = mapper;
            _db = db;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetAllCommands() =>
            Ok(_mapper.Map<IEnumerable<CommandReadDto>>(_db.Commands.ToList()));

        [HttpGet("{id}")]
        public ActionResult<CommandReadDto> GetCommandById(int id)
        {
            var cmd = _mapper.Map<CommandReadDto>(_db.Commands.FirstOrDefault(x => x.Id == id));
            if (cmd == null)
            {
                return NotFound();
            }
            return Ok(cmd);
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommand(CommandCreateDto cmd)
        {
            var mappedCmd = _mapper.Map<Command>(cmd);
            _db.Commands.Add(mappedCmd);
            _db.SaveChanges();
            return Ok(_mapper.Map<CommandReadDto>(mappedCmd));
        }

        [HttpPut("{id}")]
        public ActionResult UpdateFullCommand(int id, CommandCreateDto cmd)
        {
            var existingCmd = _db.Commands.FirstOrDefault(x => x.Id == id);
            if (existingCmd == null)
            {
                return NotFound();
            }

            _db.Commands.Update(_mapper.Map(cmd, existingCmd));
            _db.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public ActionResult UpdateCommand(int id, JsonPatchDocument<CommandUpdateDto> cmd)
        {
            var existingCmd = _db.Commands.FirstOrDefault(x => x.Id == id);
            if (existingCmd == null)
            {
                return NotFound();
            }

            var patchCmd = _mapper.Map<CommandUpdateDto>(existingCmd);
            cmd.ApplyTo(patchCmd, ModelState);

            if (!TryValidateModel(patchCmd))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(patchCmd, existingCmd);

            _db.Commands.Update(existingCmd);

            _db.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteCommandById(int id)
        {
            var cmd = _db.Commands.FirstOrDefault(x => x.Id == id);
            if (cmd == null)
            {
                return NotFound();
            }
            _db.Commands.Remove(cmd);
            _db.SaveChanges();

            return NoContent();
        }
    }
}