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

public class UBStaffLogic
{
    public Response<UBStaff> Add(UBStaff newStaff)
    {
        using var ubc = new UBContext();

        try
        {
            ubc.UBStaffs.Add(newStaff);
            ubc.SaveChanges();
            return new Response<UBStaff>()
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Payload = newStaff,
                PayloadRaw = newStaff
            };
        }
        catch (Exception ex)
        {
            return new Response<UBStaff>()
            {
                StatusCode = 400,
                StatusDescription = ex.Message
            };
        }
    }

    public Response Remove(string userId)
    {
        using var ubc = new UBContext();

        var exists = ubc.UBStaffs
            .SingleOrDefault(x => x.UBUserId == userId);
        if (exists is null)
            return new Response()
            {
                StatusCode = 404,
                StatusDescription = $"UBStaff with {userId} not found"
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

    public Response<List<UBStaff>> Get()
    {
        using var ubc = new UBContext();
        var staff = ubc.UBStaffs
            .AsNoTracking().ToList();

        return new Response<List<UBStaff>>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = staff,
            PayloadRaw = staff
        };
    }
}