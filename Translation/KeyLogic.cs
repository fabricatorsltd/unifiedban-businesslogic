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

public class KeyLogic
{
    public Response<Key> Add(string key)
    {
        using var ubc = new UBContext();

        try
        {
            var newKey = new Key()
            {
                KeyId = key
            };
            ubc.lang_Key.Add(newKey);
            ubc.SaveChanges();
            return new Response<Key>()
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Payload = newKey,
                PayloadRaw = newKey
            };
        }
        catch (Exception ex)
        {
            return new Response<Key>()
            {
                StatusCode = 400,
                StatusDescription = ex.Message
            };
        }
    }

    public Response Remove(string key)
    {
        using var ubc = new UBContext();

        var exists = ubc.lang_Key
            .AsNoTracking()
            .SingleOrDefault(x => x.KeyId == key);
        if (exists is null)
            return new Response()
            {
                StatusCode = 404,
                StatusDescription = $"Key {key} not found"
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

    public Response<List<Key>> Get()
    {
        using var ubc = new UBContext();
        var chats = ubc.lang_Key
            .AsNoTracking().ToList();

        return new Response<List<Key>>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = chats,
            PayloadRaw = chats
        };
    }
}