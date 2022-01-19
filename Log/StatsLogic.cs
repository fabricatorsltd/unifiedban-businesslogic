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
using Unifiedban.Next.Models.Log;

namespace Unifiedban.Next.BusinessLogic.Log;

public class StatsLogic
{
    private Response<Stats> Add(Stats stats)
    {
        using var ubc = new UBContext();

        try
        {
            ubc.Add(stats);
            ubc.SaveChanges();
            return new Response<Stats>()
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Payload = stats,
                PayloadRaw = stats
            };
        }
        catch (Exception ex)
        {
            return new Response<Stats>()
            {
                StatusCode = 400,
                StatusDescription = ex.Message
            };
        }
    }
    private Response<Stats> Update(Stats stats)
    {
        using var ubc = new UBContext();
        var exists = ubc.log_Stats
            .FirstOrDefault(x => x.Date == stats.Date && x.Category == stats.Category);
        if (exists is null)
            return new Response<Stats>()
            {
                StatusCode = 404,
                StatusDescription = $"Stats with category {stats.Category} for day {stats.Date} not found"
            };
            
        try
        {
            exists.Value = stats.Value;
                
            ubc.SaveChanges();
            return new Response<Stats>()
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Payload = exists,
                PayloadRaw = exists
            };
        }
        catch (Exception ex)
        {
            return new Response<Stats>()
            {
                StatusCode = 400,
                StatusDescription = ex.Message
            };
        }
    }

    public Response<Stats> AddOrUpdate(Stats stats)
    {
        using var ubc = new UBContext();
        var exists = ubc.log_Stats
            .AsNoTracking()
            .FirstOrDefault(x => x.Date == stats.Date && x.Category == stats.Category);
        return exists is null ? Add(stats) : Update(stats);
    }
        
    public Response<List<Stats>> Get()
    {
        using var ubc = new UBContext();
        var stats = ubc.log_Stats
            .AsNoTracking().ToList();

        return new Response<List<Stats>>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = stats,
            PayloadRaw = stats
        };
    }

    public Response<List<Stats>> Get(string category)
    {
        using var ubc = new UBContext();
        var stats = ubc.log_Stats
            .AsNoTracking()
            .Where(x => x.Category == category)
            .ToList();
        
        return new Response<List<Stats>>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = stats,
            PayloadRaw = stats
        };
    }
}