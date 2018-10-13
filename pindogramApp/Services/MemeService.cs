using pindogramApp.Entities;
using pindogramApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using pindogramApp.Helpers;

namespace pindogramApp.Services
{
    public class MemeService : IMemeService
    {
        private PindogramDataContext _context;

        public MemeService(PindogramDataContext context)
        {
            _context = context;
        }

        public void Downvote(int memeId, User user)
        {
            Meme meme = _context.Memes.Find(memeId);
            if (meme == null)
                throw new AppException("Requested meme does not exist.");
            MemeRate rate = _context.MemeRates.FirstOrDefault(x => x.Meme == meme && x.User == user);
            if (rate == null)
                rate = createMemeLike(meme, user);
            rate.isUpvote = false;
            _context.MemeRates.Update(rate);
            _context.SaveChanges();
        }

        public void Upvote(int memeId, User user)
        {
            Meme meme = _context.Memes.Find(memeId);
            if( meme == null)
                throw new AppException("Requested meme does not exist.");
            MemeRate rate = _context.MemeRates.FirstOrDefault(x => x.Meme == meme && x.User == user);
            if (rate == null)
                rate = createMemeLike(meme, user);
            rate.isUpvote = true;
            _context.MemeRates.Update(rate);
            _context.SaveChanges();
            
        }

        public Meme Post(string title, User author)
        {
            Meme meme = new Meme();
            meme.Title = title;
            meme.Author = author;
            meme.DateAdded = DateTime.Now;
            _context.Memes.Add(meme);
            _context.SaveChanges();

            return meme;
        }

        public int GetRate(int memeId)
        {
            int id = memeId;
            var votes = _context.MemeRates.Where(m => m.MemeId == memeId);
            int upvotedCount = votes.Count(x => x.isUpvote == true);
            int downvotedCount = votes.Count(x => x.isUpvote == false);
            return upvotedCount - downvotedCount;
        }

        //helpers

        private MemeRate createMemeLike(Meme meme, User user)
        {

            MemeRate newLike = new MemeRate();
            newLike.Meme = meme;
            newLike.User = user;
            _context.MemeRates.Add(newLike);
            _context.SaveChanges();
            return newLike;
        }

 
    }
}
