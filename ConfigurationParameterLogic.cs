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

public class ConfigurationParameterLogic
{
    public Response<ConfigurationParameter> Add(ConfigurationParameter configParam)
    {
        using var ubc = new UBContext();

        try
        {
            ubc.ConfigurationParameters.Add(configParam);
            ubc.SaveChanges();
            return new Response<ConfigurationParameter>()
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Payload = configParam,
                PayloadRaw = configParam
            };
        }
        catch (Exception ex)
        {
            return new Response<ConfigurationParameter>()
            {
                StatusCode = 400,
                StatusDescription = ex.Message
            };
        }
    }

    public Response Remove(string paramId)
    {
        using var ubc = new UBContext();

        var exists = ubc.ConfigurationParameters
            .SingleOrDefault(x => x.ConfigurationParameterId == paramId);
        if (exists is null)
            return new Response()
            {
                StatusCode = 404,
                StatusDescription = $"ConfigurationParameter with {paramId} not found"
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

    public Response<List<ConfigurationParameter>> Get()
    {
        using var ubc = new UBContext();
        var chats = ubc.ConfigurationParameters
            .AsNoTracking().ToList();

        return new Response<List<ConfigurationParameter>>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = chats,
            PayloadRaw = chats
        };
    }
    public Response<List<ConfigurationParameter>> GetByPlatform(string platform)
    {
        using var ubc = new UBContext();
        var configs = ubc.ConfigurationParameters
            .AsNoTracking()
            .ToList()
            .Where(x => ContainsPlatform(x.Platforms, platform))
            .ToList();

        return new Response<List<ConfigurationParameter>>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = configs
        };
    }

    private bool ContainsPlatform(IEnumerable<string> source, string platform)
    {
        return source.Contains(platform);
    }
}