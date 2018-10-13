using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pindogramApp.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AutoMapper;
using pindogramApp.Services.Interfaces;
using pindogramApp.Helpers;
using pindogramApp.Dtos;

namespace pindogramApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MemesController : ControllerBase
    {
        private readonly PindogramDataContext _context;
        private IMemeService _memeService;
        private IMapper _mapper;

        public MemesController(PindogramDataContext context, IMapper mapper, IMemeService MemeService)
        {
            _memeService = MemeService;
            _context = context;
        }

        [HttpPost("postmeme")]
        public async Task<IActionResult> Post(string title)
        {

            User loggedUser = getLoggedUser();
            if (loggedUser == null)
            {
                return BadRequest(new { message = "Not logged in" });
            }
            if (String.IsNullOrEmpty(title))
            {
                return BadRequest(new { message = "Cannot add untitled meme" });
            }
            try
            {
                // save 
                _memeService.Post(title, loggedUser);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("upvote")]
        public async Task<IActionResult> Upvote(int memeId)
        {
            User loggedUser = getLoggedUser();
            try
            {
                _memeService.Upvote(memeId, loggedUser);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpPost("downvote")]
        public async Task<IActionResult> Downvote(int memeId)
        {
            User loggedUser = getLoggedUser();
            try
            {
                _memeService.Downvote(memeId, loggedUser);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IEnumerable<Meme> GetMemes()
        {
            return _context.Memes;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMeme([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var meme = await _context.Memes.FindAsync(id);

            if (meme == null)
            {
                return NotFound();
            }

            return Ok(meme);
        }

        [AllowAnonymous]
        [HttpGet("getRate")]
        public async Task<IActionResult> GetRate(int memeId)
        {
            
            try
            {
                int rate = 0;
                rate = _memeService.GetRate(memeId);
                return Ok(rate);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
        // DELETE: api/Memes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeme([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var meme = await _context.Memes.FindAsync(id);
            if (meme == null)
            {
                return NotFound();
            }

            _context.Memes.Remove(meme);
            await _context.SaveChangesAsync();

            return Ok(meme);
        }

        private bool MemeExists(int id)
        {
            return _context.Memes.Any(e => e.Id == id);
        }

        private User getLoggedUser()
        {
            string strAutId = this.User.FindFirst(ClaimTypes.Name).Value;
            int autId = int.Parse(strAutId);
            User loggedUser = _context.Users.First(x => x.Id == autId);
            return loggedUser;
        }
    }
}