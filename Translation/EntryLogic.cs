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
using Unifiedban.Next.Models.Translation;

namespace Unifiedban.Next.BusinessLogic.Translation;

public class EntryLogic
{
    public Response<Entry> Add(Entry entry)
    {
        using var ubc = new UBContext();

        try
        {
            ubc.lang_Entries.Add(entry);
            ubc.SaveChanges();
            return new Response<Entry>()
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Payload = entry,
                PayloadRaw = entry
            };
        }
        catch (Exception ex)
        {
            return new Response<Entry>()
            {
                StatusCode = 400,
                StatusDescription = ex.Message
            };
        }
    }

    public Response Remove(string keyId, string langId)
    {
        using var ubc = new UBContext();

        var exists = ubc.lang_Entries
            .AsNoTracking()
            .SingleOrDefault(x => x.KeyId == keyId && x.LanguageId == langId);
        if (exists is null)
            return new Response()
            {
                StatusCode = 404,
                StatusDescription = $"Entry with {keyId} for LanguageId {langId} not found"
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

    public Response<Entry> Update(Entry entry)
    {
        using var ubc = new UBContext();

        var exists = ubc.lang_Entries
            .SingleOrDefault(x => x.KeyId == entry.KeyId && x.LanguageId == entry.LanguageId);
        if (exists is null)
            return new Response<Entry>()
            {
                StatusCode = 404,
                StatusDescription = $"Entry with {entry.KeyId} for LanguageId {entry.LanguageId} not found"
            };

        try
        {
            exists.Translation = entry.Translation;

            ubc.SaveChanges();
            return new Response<Entry>()
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Payload = exists,
                PayloadRaw = exists
            };
        }
        catch (Exception ex)
        {
            return new Response<Entry>()
            {
                StatusCode = 400,
                StatusDescription = ex.Message
            };
        }
    }

    public Response<List<Entry>> Get()
    {
        using var ubc = new UBContext();
        var chats = ubc.lang_Entries
            .AsNoTracking().ToList();

        return new Response<List<Entry>>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = chats,
            PayloadRaw = chats
        };
    }
}