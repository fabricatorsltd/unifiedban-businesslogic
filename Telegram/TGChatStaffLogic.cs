/* unified/ban - Management and protection systems

© fabricators SRL, https://fabricators.ltd , https://unifiedban.solutions

This program is free software: you can redistribute it and/or modify
it under the terms of the fabricator's FOSS License.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the fabricator's FOSS License
along with this program.  If not, see <https://fabricators.ltd/FOSSLicense>. */

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unifiedban.Next.Common;
using Unifiedban.Next.Models;
using Unifiedban.Next.Models.Telegram;

namespace Unifiedban.Next.BusinessLogic.Telegram;

public class TGChatMemberLogic
{
    public Response<TGChatMember> Add(TGChatMember tgChat)
    {
        using var ubc = new UBContext();

        try
        {
            ubc.Add(tgChat);
            ubc.SaveChanges();
            return new Response<TGChatMember>()
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Payload = tgChat,
                PayloadRaw = tgChat
            };
        }
        catch (Exception ex)
        {
            return new Response<TGChatMember>()
            {
                StatusCode = 400,
                StatusDescription = ex.Message
            };
        }
    }

    public Response Remove(string ubChatId, string ubUserId)
    {
        using var ubc = new UBContext();

        var exists = ubc.TGChatStaffs
            .SingleOrDefault(x => x.ChatId == ubChatId && x.UBUserId == ubUserId);
        if (exists is null)
            return new Response()
            {
                StatusCode = 404,
                StatusDescription = $"TGChatStaff with  ChatId {ubChatId} not found"
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

    public Response<TGChatMember> Update(TGChatMember tgStaff)
    {
        using var ubc = new UBContext();

        var exists = ubc.TGChatStaffs
            .SingleOrDefault(x => x.ChatId == tgStaff.ChatId && x.UBUserId == tgStaff.UBUserId);
        if (exists is null)
            return new Response<TGChatMember>()
            {
                StatusCode = 404,
                StatusDescription =
                    $"TGChatStaff with  ChatId {tgStaff.ChatId} and USerId {tgStaff.UBUserId} not found"
            };

        try
        {
            exists.StaffType = tgStaff.StaffType;

            ubc.SaveChanges();
            return new Response<TGChatMember>()
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Payload = tgStaff,
                PayloadRaw = tgStaff
            };
        }
        catch (Exception ex)
        {
            return new Response<TGChatMember>()
            {
                StatusCode = 400,
                StatusDescription = ex.Message
            };
        }
    }

    public Response<List<TGChatMember>> Get()
    {
        using var ubc = new UBContext();
        var tgChats = ubc.TGChatStaffs
            .AsNoTracking().ToList();

        return new Response<List<TGChatMember>>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = tgChats,
            PayloadRaw = tgChats
        };
    }

    public Response<TGChatMember> Get(string ubChatId)
    {
        using var ubc = new UBContext();
        var tgChat = ubc.TGChatStaffs
            .AsNoTracking()
            .SingleOrDefault(x => x.ChatId == ubChatId);

        if (tgChat is null)
            return new Response<TGChatMember>()
            {
                StatusCode = 404,
                StatusDescription = $"TGChatStaff with  ChatId {ubChatId} not found"
            };

        return new Response<TGChatMember>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = tgChat,
            PayloadRaw = tgChat
        };
    }
}