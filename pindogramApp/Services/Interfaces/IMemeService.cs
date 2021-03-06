﻿using pindogramApp.Entities;
using System.Collections.Generic;
using pindogramApp.Dtos;


namespace pindogramApp.Services.Interfaces
{
    public interface IMemeService
    {
        Meme Create(CreateMemeDto title, User author);
        void Upvote(int memeId, User user);
        void Downvote(int memeId, User user);
        User GetMemeAuthor(int memeId);
        IEnumerable<Meme> GetAllApproved();
        IEnumerable<Meme> GetAllUnapproved();
        Meme GetSingleApprovedById(int id);
        Meme GetSingleUnapprovedById(int id);
        string ApproveMeme(int id);
        void Delete(int id);
        int GetRate(int memeId);
        User GetLoggedUser(string strAutId);
        bool IsActiveUp(int memeId, int userId);
        bool IsActiveDown(int memeId, int userId);
        IEnumerable<TotalNumberOfLikesDislikesDto> GetTotalNumerOfLikesAndDislikes();
        IEnumerable<DateToApprovedMemesDto> GetDateToNumberOfApprovedMemes();
    }
}
