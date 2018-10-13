using pindogramApp.Entities;
using System.Collections.Generic;



namespace pindogramApp.Services.Interfaces
{
    public interface IMemeService
    {
        Meme Post(string title, User author);
        void Upvote(int memeId, User user);
        void Downvote(int memeId, User user);
        int GetRate(int memeId);
    }
}
