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

namespace Unifiedban.Next.BusinessLogic;

public class UBUserLogic
{
    private Response<UBUser> Add(UBUser user)
    {
        using var ubc = new UBContext();

        try
        {
            ubc.UBUsers.Add(user);
            ubc.SaveChanges();
            return new Response<UBUser>()
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Payload = user,
                PayloadRaw = user
            };
        }
        catch (Exception ex)
        {
            return new Response<UBUser>()
            {
                StatusCode = 400,
                StatusDescription = ex.Message
            };
        }
    }
    public Response<UBUser> AddIfNoneFound(UBUser user)
    {
        using var ubc = new UBContext();

        if (!string.IsNullOrWhiteSpace(user.FabUserId))
        {
            var fabIdSearch = ubc.UBUsers
                .AsNoTracking()
                .SingleOrDefault(x => x.FabUserId == user.FabUserId);
            if (fabIdSearch is not null)
            {
                fabIdSearch.TelegramId ??= user.TelegramId;
                fabIdSearch.DiscordId ??= user.DiscordId;
                fabIdSearch.TwitchId ??= user.TwitchId;
                return Update(fabIdSearch);
            }
        }

        if (user.TelegramId is not null)
        {
            var telegramSearch = ubc.UBUsers
                .AsNoTracking()
                .SingleOrDefault(x => x.TelegramId == user.TelegramId);
            if (telegramSearch is not null)
            {
                telegramSearch.FabUserId ??= user.FabUserId;
                telegramSearch.DiscordId ??= user.DiscordId;
                telegramSearch.TwitchId ??= user.TwitchId;
                return Update(telegramSearch);
            }
        }

        if (user.DiscordId is not null)
        {
            var discordSearch = ubc.UBUsers
                .AsNoTracking()
                .SingleOrDefault(x => x.DiscordId == user.DiscordId);
            if (discordSearch is not null)
            {
                discordSearch.FabUserId ??= user.FabUserId;
                discordSearch.TelegramId ??= user.TelegramId;
                discordSearch.TwitchId ??= user.TwitchId;
                return Update(discordSearch);
            }
        }

        if (user.TwitchId is not null)
        {
            var twitchSearch = ubc.UBUsers
                .AsNoTracking()
                .SingleOrDefault(x => x.TwitchId == user.TwitchId);
            if (twitchSearch is not null)
            {
                twitchSearch.FabUserId ??= user.FabUserId;
                twitchSearch.TelegramId ??= user.TelegramId;
                twitchSearch.DiscordId ??= user.DiscordId;
                return Update(twitchSearch);
            }
        }

        return Add(user);
    }
    public Response<UBUser> Update(UBUser user)
    {
        using var ubc = new UBContext();

        var exists = ubc.UBUsers
            .SingleOrDefault(x => x.UBUserId == user.UBUserId);
        if (exists is null)
            return new Response<UBUser>()
            {
                StatusCode = 404,
                StatusDescription = $"UBUser with {user.UBUserId} not found"
            };

        try
        {
            exists.TelegramId = user.TelegramId;
            exists.DiscordId = user.DiscordId;
            exists.TwitchId = user.TwitchId;
            exists.TrustFactor = user.TrustFactor;
            ubc.SaveChanges();
            return new Response<UBUser>()
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Payload = exists,
                PayloadRaw = exists
            };
        }
        catch (Exception ex)
        {
            return new Response<UBUser>()
            {
                StatusCode = 400,
                StatusDescription = ex.Message
            };
        }
    }

    public Response Remove(string userId)
    {
        using var ubc = new UBContext();

        var exists = ubc.UBUsers
            .SingleOrDefault(x => x.UBUserId == userId);
        if (exists is null)
            return new Response()
            {
                StatusCode = 404,
                StatusDescription = $"UBUser with {userId} not found"
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

    public Response<UBUser> MergeFabTelegram(string fabricatorsId, long telegramId)
    {
        var userFab = GetByFabricatorsId(fabricatorsId);
        if (userFab.StatusCode != 200)
        {
            return new Response<UBUser>()
            {
                StatusCode = 404,
                StatusDescription = $"Can't find user with FabricatorsId {fabricatorsId}"
            };
        }
            
        var userTg = GetByTelegramId(telegramId);
        if (userTg.StatusCode != 200)
        {
            return new Response<UBUser>()
            {
                StatusCode = 404,
                StatusDescription = $"Can't find user with TelegramId {telegramId}"
            };
        }

        if (userTg.Payload.Count != 1)
        {
            return new Response<UBUser>()
            {
                StatusCode = 400,
                StatusDescription = $"More than one user found with TelegramId {telegramId}"
            };
        }

        userFab.Payload.TelegramId = userTg.Payload[0].TelegramId;
        userFab.Payload.DiscordId ??= userTg.Payload[0].DiscordId;
        userFab.Payload.TwitchId ??= userTg.Payload[0].TwitchId;

        userTg.Payload[0].State = Enums.UserStates.Merged;
        Update(userTg.Payload[0]);
            
        return Update(userFab.Payload);
    }

    public Response<UBUser> MergeFabDiscord(string fabricatorsId, long discordId)
    {
        var userFab = GetByFabricatorsId(fabricatorsId);
        if (userFab.StatusCode != 200)
        {
            return new Response<UBUser>()
            {
                StatusCode = 404,
                StatusDescription = $"Can't find user with FabricatorsId {fabricatorsId}"
            };
        }
            
        var userDs = GetByDiscordId(discordId);
        if (userDs.StatusCode != 200)
        {
            return new Response<UBUser>()
            {
                StatusCode = 404,
                StatusDescription = $"Can't find user with DiscordId {discordId}"
            };
        }

        if (userDs.Payload.Count != 1)
        {
            return new Response<UBUser>()
            {
                StatusCode = 400,
                StatusDescription = $"More than one user found with DiscordId {discordId}"
            };
        }

        userFab.Payload.TelegramId ??= userDs.Payload[0].TelegramId;
        userFab.Payload.DiscordId = userDs.Payload[0].DiscordId;
        userFab.Payload.TwitchId ??= userDs.Payload[0].TwitchId;

        userDs.Payload[0].State = Enums.UserStates.Merged;
        Update(userDs.Payload[0]);
            
        return Update(userFab.Payload);
    }

    public Response<UBUser> MergeFabTwitch(string fabricatorsId, long twitchId)
    {
        var userFab = GetByFabricatorsId(fabricatorsId);
        if (userFab.StatusCode != 200)
        {
            return new Response<UBUser>()
            {
                StatusCode = 404,
                StatusDescription = $"Can't find user with FabricatorsId {fabricatorsId}"
            };
        }
            
        var userTw = GetByTwitchId(twitchId);
        if (userTw.StatusCode != 200)
        {
            return new Response<UBUser>()
            {
                StatusCode = 404,
                StatusDescription = $"Can't find user with TwitchId {twitchId}"
            };
        }

        if (userTw.Payload.Count != 1)
        {
            return new Response<UBUser>()
            {
                StatusCode = 400,
                StatusDescription = $"More than one user found with TwitchId {twitchId}"
            };
        }

        userFab.Payload.TelegramId ??= userTw.Payload[0].TelegramId;
        userFab.Payload.DiscordId ??= userTw.Payload[0].DiscordId;
        userFab.Payload.TwitchId = userTw.Payload[0].TwitchId;

        userTw.Payload[0].State = Enums.UserStates.Merged;
        Update(userTw.Payload[0]);
            
        return Update(userFab.Payload);
    }
        
        
    public Response<List<UBUser>> Get()
    {
        using var ubc = new UBContext();
        var users = ubc.UBUsers
            .AsNoTracking().ToList();

        return new Response<List<UBUser>>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = users,
            PayloadRaw = users
        };
    }

    public Response<UBUser> GetByFabricatorsId(string fabricatorsId)
    {
        using var ubc = new UBContext();
        var user = ubc.UBUsers
            .AsNoTracking()
            .FirstOrDefault(x => x.FabUserId == fabricatorsId);
            
        if(user is null)
            return new Response<UBUser>()
            {
                StatusCode = 404,
                StatusDescription = "User not found"
            };

        return new Response<UBUser>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = user,
            PayloadRaw = user
        };
    }
    public Response<List<UBUser>> GetByTelegramId(long telegramId)
    {
        using var ubc = new UBContext();
        var users = ubc.UBUsers
            .AsNoTracking()
            .Where(x => x.TelegramId == telegramId)
            .ToList();
            
        if(users.Count == 0)
            return new Response<List<UBUser>>()
            {
                StatusCode = 404,
                StatusDescription = "User not found"
            };

        return new Response<List<UBUser>>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = users,
            PayloadRaw = users
        };
    }

    public Response<List<UBUser>> GetByDiscordId(long discordId)
    {
        using var ubc = new UBContext();
        var users = ubc.UBUsers
            .AsNoTracking()
            .Where(x => x.DiscordId == discordId)
            .ToList();
            
        if(users.Count == 0)
            return new Response<List<UBUser>>()
            {
                StatusCode = 404,
                StatusDescription = "User not found"
            };

        return new Response<List<UBUser>>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = users,
            PayloadRaw = users
        };
    }

    public Response<List<UBUser>> GetByTwitchId(long twitchId)
    {
        using var ubc = new UBContext();
        var users = ubc.UBUsers
            .AsNoTracking()
            .Where(x => x.TwitchId == twitchId)
            .ToList();
            
        if(users.Count == 0)
            return new Response<List<UBUser>>()
            {
                StatusCode = 404,
                StatusDescription = "User not found"
            };

        return new Response<List<UBUser>>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = users,
            PayloadRaw = users
        };
    }
}