﻿namespace ESchoolBot
{
    public interface IDatabaseClient
    {
        void InsertUser(long chatId, string username, string password, string sessionId);
    }
}