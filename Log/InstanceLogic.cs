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

public class InstanceLogic
{
    public Response<Instance> Add(Instance instance)
    {
        using var ubc = new UBContext();

        try
        {
            ubc.Add(instance);
            ubc.SaveChanges();
            return new Response<Instance>()
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Payload = instance,
                PayloadRaw = instance
            };
        }
        catch (Exception ex)
        {
            return new Response<Instance>()
            {
                StatusCode = 400,
                StatusDescription = ex.Message
            };
        }
    }

    public Response<Instance> Update(Instance instance)
    {
        using var ubc = new UBContext();
        var exists = ubc.log_Instances
            .FirstOrDefault(x => x.InstanceId == instance.InstanceId);
        if (exists is null)
            return new Response<Instance>()
            {
                StatusCode = 404,
                StatusDescription = $"Instance with InstanceId {instance.InstanceId} not found"
            };
            
        try
        {
            exists.Status = instance.Status;
            exists.Stop = instance.Stop;
                
            ubc.SaveChanges();
            return new Response<Instance>()
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Payload = instance,
                PayloadRaw = instance
            };
        }
        catch (Exception ex)
        {
            return new Response<Instance>()
            {
                StatusCode = 400,
                StatusDescription = ex.Message
            };
        }
    }
        
    public Response<List<Instance>> Get()
    {
        using var ubc = new UBContext();
        var instances = ubc.log_Instances
            .AsNoTracking().ToList();

        return new Response<List<Instance>>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = instances,
            PayloadRaw = instances
        };
    }

    public Response<Instance> Get(string instanceId)
    {
        using var ubc = new UBContext();
        var instance = ubc.log_Instances
            .AsNoTracking()
            .SingleOrDefault(x => x.InstanceId == instanceId);

        if (instance is null)
            return new Response<Instance>()
            {
                StatusCode = 404,
                StatusDescription = $"Instance with InstanceId {instanceId} not found"
            };

        return new Response<Instance>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = instance,
            PayloadRaw = instance
        };
    }
}