/* unified/ban - Management and protection systems

© fabricators SRL, https://fabricators.ltd , https://unifiedban.solutions

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License with our addition
to Section 7 as published in unified/ban's the GitHub repository.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License and the
additional terms along with this program. 
If not, see <https://docs.fabricators.ltd/docs/licenses/unifiedban>.

For more information, see Licensing FAQ: 

https://docs.fabricators.ltd/docs/licenses/faq */

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unifiedban.Next.Common;
using Unifiedban.Next.Models;

namespace Unifiedban.Next.BusinessLogic;

public class UBChatLogic
{
    public Response<UBChat> Add()
    {
        using var ubc = new UBContext();

        try
        {
            var newChat = new UBChat();
            ubc.UBChats.Add(newChat);
            ubc.SaveChanges();
            return new Response<UBChat>()
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Payload = newChat,
                PayloadRaw = newChat
            };
        }
        catch (Exception ex)
        {
            return new Response<UBChat>()
            {
                StatusCode = 400,
                StatusDescription = ex.Message
            };
        }
    }

    public Response Remove(string chatId)
    {
        using var ubc = new UBContext();

        var exists = ubc.UBChats
            .AsNoTracking()
            .SingleOrDefault(x => x.UBChatId == chatId);
        if (exists is null)
            return new Response()
            {
                StatusCode = 404,
                StatusDescription = $"UBChat with {chatId} not found"
            };

        try
        {
            ubc.Remove(exists);
            ubc.SaveChanges();
            return new Response()
            {
                StatusCode = 200,
                StatusDescription = "OK"
            };
        }
        catch (Exception ex)
        {
            return new Response()
            {
                StatusCode = 400,
                StatusDescription = ex.Message
            };
        }
    }

    public Response<List<UBChat>> Get()
    {
        using var ubc = new UBContext();
        var chats = ubc.UBChats
            .AsNoTracking().ToList();

        return new Response<List<UBChat>>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = chats,
            PayloadRaw = chats
        };
    }
}