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

public class LanguageLogic
{
    public Response<Language> Add(Language lang)
    {
        using var ubc = new UBContext();

        try
        {
            ubc.lang_Languages.Add(lang);
            ubc.SaveChanges();
            return new Response<Language>()
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Payload = lang,
                PayloadRaw = lang
            };
        }
        catch (Exception ex)
        {
            return new Response<Language>()
            {
                StatusCode = 400,
                StatusDescription = ex.Message
            };
        }
    }

    public Response Remove(string lang)
    {
        using var ubc = new UBContext();

        var exists = ubc.lang_Languages
            .AsNoTracking()
            .SingleOrDefault(x => x.LanguageId == lang);
        if (exists is null)
            return new Response()
            {
                StatusCode = 404,
                StatusDescription = $"Language {lang} not found"
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

    public Response<Language> Update(Language lang)
    {
        using var ubc = new UBContext();

        var exists = ubc.lang_Languages
            .SingleOrDefault(x => x.LanguageId == lang.LanguageId);
        if (exists is null)
            return new Response<Language>()
            {
                StatusCode = 404,
                StatusDescription = $"Language with  LanguageId {lang.LanguageId} not found"
            };

        try
        {
            exists.Name = lang.Name;
            exists.Status = lang.Status;
            exists.UniversalCode = lang.UniversalCode;

            ubc.SaveChanges();
            return new Response<Language>()
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Payload = exists,
                PayloadRaw = exists
            };
        }
        catch (Exception ex)
        {
            return new Response<Language>()
            {
                StatusCode = 400,
                StatusDescription = ex.Message
            };
        }
    }

    public Response<List<Language>> Get()
    {
        using var ubc = new UBContext();
        var chats = ubc.lang_Languages
            .AsNoTracking().ToList();

        return new Response<List<Language>>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = chats,
            PayloadRaw = chats
        };
    }
}