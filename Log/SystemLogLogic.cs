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
using Unifiedban.Next.Models.Log;

namespace Unifiedban.Next.BusinessLogic.Log;

public class SystemLogLogic
{
    public Response<SystemLog> Add(SystemLog sysLog)
    {
        using var ubc = new UBContext();

        try
        {
            ubc.Add(sysLog);
            ubc.SaveChanges();
            return new Response<SystemLog>()
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Payload = sysLog,
                PayloadRaw = sysLog
            };
        }
        catch (Exception ex)
        {
            return new Response<SystemLog>()
            {
                StatusCode = 400,
                StatusDescription = ex.Message
            };
        }
    }

    public Response<List<SystemLog>> Get()
    {
        using var ubc = new UBContext();
        var sysLogs = ubc.log_SystemLogs
            .AsNoTracking().ToList();

        return new Response<List<SystemLog>>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = sysLogs,
            PayloadRaw = sysLogs
        };
    }
}