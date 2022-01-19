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

public class TGUserLogic
{
    public Response<TGUser> Add(TGUser tgUser)
    {
        using var ubc = new UBContext();

        try
        {
            ubc.Add(tgUser);
            ubc.SaveChanges();
            return new Response<TGUser>()
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Payload = tgUser,
                PayloadRaw = tgUser
            };
        }
        catch (Exception ex)
        {
            return new Response<TGUser>()
            {
                StatusCode = 400,
                StatusDescription = ex.Message
            };
        }
    }

    public Response Remove(string ubUserId)
    {
        using var ubc = new UBContext();

        var exists = ubc.TGUsers
            .SingleOrDefault(x => x.UBUserId == ubUserId);
        if (exists is null)
            return new Response()
            {
                StatusCode = 404,
                StatusDescription = $"TGUser with  UBUserId {ubUserId} not found"
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

    public Response<TGUser> Update(TGUser tgUser)
    {
        using var ubc = new UBContext();

        var exists = ubc.TGUsers
            .SingleOrDefault(x => x.UBUserId == tgUser.UBUserId);
        if (exists is null)
            return new Response<TGUser>()
            {
                StatusCode = 404,
                StatusDescription = $"TGUser with  UBUserId {tgUser.UBUserId} not found"
            };

        try
        {
            exists.TrustFactor = tgUser.TrustFactor;

            ubc.SaveChanges();
            return new Response<TGUser>()
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Payload = tgUser,
                PayloadRaw = tgUser
            };
        }
        catch (Exception ex)
        {
            return new Response<TGUser>()
            {
                StatusCode = 400,
                StatusDescription = ex.Message
            };
        }
    }

    public Response<List<TGUser>> Get()
    {
        using var ubc = new UBContext();
        var tgUsers = ubc.TGUsers
            .AsNoTracking().ToList();

        return new Response<List<TGUser>>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = tgUsers,
            PayloadRaw = tgUsers
        };
    }

    public Response<TGUser> Get(string ubUserId)
    {
        using var ubc = new UBContext();
        var tgUser = ubc.TGUsers
            .AsNoTracking()
            .SingleOrDefault(x => x.UBUserId == ubUserId);

        if (tgUser is null)
            return new Response<TGUser>()
            {
                StatusCode = 404,
                StatusDescription = $"TGUser with  UBUserId {ubUserId} not found"
            };

        return new Response<TGUser>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = tgUser,
            PayloadRaw = tgUser
        };
    }

    public Response<TGUser> Get(long telegramId)
    {
        using var ubc = new UBContext();
        var tgUser = ubc.TGUsers
            .AsNoTracking()
            .Include(x => x.UBUser)
            .SingleOrDefault(x => x.UBUser.TelegramId == telegramId);

        if (tgUser is null)
            return new Response<TGUser>()
            {
                StatusCode = 404,
                StatusDescription = $"TGUser with  TelegramId {telegramId} not found"
            };

        return new Response<TGUser>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = tgUser,
            PayloadRaw = tgUser
        };
    }
}