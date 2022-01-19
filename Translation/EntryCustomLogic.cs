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
using Unifiedban.Next.Models.Translation;

namespace Unifiedban.Next.BusinessLogic.Translation;

public class EntryCustomCustomLogic
{
    public Response<EntryCustom> Add(EntryCustom entry)
    {
        using var ubc = new UBContext();

        try
        {
            ubc.lang_CustomEntries.Add(entry);
            ubc.SaveChanges();
            return new Response<EntryCustom>()
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Payload = entry,
                PayloadRaw = entry
            };
        }
        catch (Exception ex)
        {
            return new Response<EntryCustom>()
            {
                StatusCode = 400,
                StatusDescription = ex.Message
            };
        }
    }

    public Response Remove(string keyId, string langId, string chatId)
    {
        using var ubc = new UBContext();

        var exists = ubc.lang_CustomEntries
            .AsNoTracking()
            .SingleOrDefault(x => x.KeyId == keyId && x.LanguageId == langId && x.ChatId == chatId);
        if (exists is null)
            return new Response()
            {
                StatusCode = 404,
                StatusDescription = $"EntryCustom with {keyId} for LanguageId {langId} not found"
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

    public Response<EntryCustom> Update(EntryCustom entry)
    {
        using var ubc = new UBContext();

        var exists = ubc.lang_CustomEntries
            .SingleOrDefault(x => x.KeyId == entry.KeyId && x.LanguageId == entry.LanguageId &&
                                  x.ChatId == entry.ChatId);
        if (exists is null)
            return new Response<EntryCustom>()
            {
                StatusCode = 404,
                StatusDescription = $"EntryCustom with {entry.KeyId} for LanguageId {entry.LanguageId} not found"
            };

        try
        {
            exists.Translation = entry.Translation;

            ubc.SaveChanges();
            return new Response<EntryCustom>()
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Payload = exists,
                PayloadRaw = exists
            };
        }
        catch (Exception ex)
        {
            return new Response<EntryCustom>()
            {
                StatusCode = 400,
                StatusDescription = ex.Message
            };
        }
    }

    public Response<List<EntryCustom>> Get()
    {
        using var ubc = new UBContext();
        var chats = ubc.lang_CustomEntries
            .AsNoTracking().ToList();

        return new Response<List<EntryCustom>>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = chats,
            PayloadRaw = chats
        };
    }

    public Response<List<EntryCustom>> Get(string chatId)
    {
        using var ubc = new UBContext();
        var chats = ubc.lang_CustomEntries
            .AsNoTracking()
            .Where(x => x.ChatId == chatId)
            .ToList();

        return new Response<List<EntryCustom>>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = chats,
            PayloadRaw = chats
        };
    }
}