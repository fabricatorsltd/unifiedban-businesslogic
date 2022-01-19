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

public class ModuleLogic
{
    public Response<Module> Add(Module module)
    {
        using var ubc = new UBContext();

        try
        {
            ubc.Add(module);
            ubc.SaveChanges();
            return new Response<Module>()
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Payload = module,
                PayloadRaw = module
            };
        }
        catch (Exception ex)
        {
            return new Response<Module>()
            {
                StatusCode = 400,
                StatusDescription = ex.Message
            };
        }
    }
    
    public Response Update(Module module)
    {
        using var ubc = new UBContext();

        var exists = ubc.Modules
            .SingleOrDefault(x => x.ModuleId == module.ModuleId);
        if (exists is null)
            return new Response()
            {
                StatusCode = 404,
                StatusDescription = $"Module {module.ModuleId} not found"
            };

        try
        {
            exists.Status = module.Status;
            exists.Version = module.Version;
            exists.Exchange = module.Exchange;
            exists.RoutingKey = module.RoutingKey;
            exists.QueueName = module.QueueName;
            exists.ChainPriority = module.ChainPriority;
            exists.Platforms = module.Platforms;
            exists.MessageCategory = module.MessageCategory;
            
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
    
    public Response Remove(string moduleId)
    {
        using var ubc = new UBContext();

        var exists = ubc.Modules
            .SingleOrDefault(x => x.ModuleId == moduleId);
        if (exists is null)
            return new Response()
            {
                StatusCode = 404,
                StatusDescription = $"Module {moduleId} not found"
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
    
    public Response<Module> GetModule(string moduleId)
    {
        using var ubc = new UBContext();
        var module =ubc.Modules
            .AsNoTracking()
            .SingleOrDefault(x => x.ModuleId == moduleId);
            
        if(module is null)
            return new Response<Module>()
            {
                StatusCode = 404,
                StatusDescription = $"Module {moduleId} not found"
            };

        return new Response<Module>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = module,
            PayloadRaw = module
        };
    }
    public Response<List<Module>> GetModules(bool inChainOrder = true)
    {
        using var ubc = new UBContext();

        List<Module> modules;
        
        if (inChainOrder)
        {
            modules = ubc.Modules
                .AsNoTracking()
                .Where(x => x.Status == Enums.States.Operational)
                .OrderBy(x => x.ChainPriority)
                .ToList();
        }
        else
        {
            modules = ubc.Modules
                .AsNoTracking()
                .Where(x => x.Status == Enums.States.Operational)
                .ToList();
        }

        return new Response<List<Module>>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = modules,
            PayloadRaw = modules
        };
    }
    public Response<List<Module>> GetModules(Enums.QueueMessageCategories category, bool inChainOrder = true)
    {
        using var ubc = new UBContext();

        List<Module> modules;
        
        if (inChainOrder)
        {
            modules = ubc.Modules
                .AsNoTracking()
                .Where(x => x.Status == Enums.States.Operational && x.MessageCategory == category)
                .OrderBy(x => x.ChainPriority)
                .ToList();
        }
        else
        {
            modules = ubc.Modules
                .AsNoTracking()
                .Where(x => x.Status == Enums.States.Operational && x.MessageCategory == category)
                .ToList();
        }

        return new Response<List<Module>>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = modules,
            PayloadRaw = modules
        };
    }
    
    public Response<List<Module>> GetModules(string platform, bool inChainOrder = true)
    {
        using var ubc = new UBContext();

        List<Module> modules;
        
        if (inChainOrder)
        {
            modules = ubc.Modules
                .AsNoTracking()
                .Where(x => x.Platforms.Contains(platform))
                .OrderBy(x => x.ChainPriority)
                .ToList();
        }
        else
        {
            modules = ubc.Modules
                .AsNoTracking()
                .Where(x => x.Platforms.Contains(platform))
                .ToList();
        }

        return new Response<List<Module>>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = modules,
            PayloadRaw = modules
        };
    }
}