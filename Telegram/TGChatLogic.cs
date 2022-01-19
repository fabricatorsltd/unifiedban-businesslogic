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
using Newtonsoft.Json;
using Unifiedban.Next.Common;
using Unifiedban.Next.Models;
using Unifiedban.Next.Models.Telegram;

namespace Unifiedban.Next.BusinessLogic.Telegram;

public class TGChatLogic
{
    private readonly UBChatLogic _ubChatLogic = new();
    private readonly UBUserLogic _ubUserLogic = new();
    private readonly ConfigurationParameterLogic _configurationParameterLogic = new();
    private readonly TGChatMemberLogic _tgChatMemberLogic = new();
        
    public Response<TGChat> Register(
        long telegramChatId,
        string title,
        long reportChatId,
        string lang,
        long creator)
    {
        var ubUser = _ubUserLogic.AddIfNoneFound(new UBUser {TelegramId = creator});
        if (ubUser.StatusCode != 0)
        {
            return new Response<TGChat>()
            {
                StatusCode = 400,
                StatusDescription = $"Error creating UBUser profile for Telegram userId {creator}"
            };
        }

        var ubChat = _ubChatLogic.Add();
        if (ubChat.StatusCode != 0)
        {
            return new Response<TGChat>()
            {
                StatusCode = 400,
                StatusDescription = $"Error creating UBChat: {ubChat.StatusDescription}"
            };
        }

        var jsonConfig = JsonConvert.SerializeObject(_configurationParameterLogic.Get());
            
        // TODO - check if language exists and fallback to first half id contains - or to English as global default
            
        var tgChat = Add(new TGChat
        {
            ChatId = ubChat.Payload.UBChatId,
            Status = Enums.ChatStates.Active,
            TelegramChatId = telegramChatId,
            OwnerId = ubUser.Payload.UBUserId,
            Title = title,
            WelcomeText = string.Empty,
            RulesText = string.Empty,
            SettingsLanguage = lang,
            Configuration = jsonConfig,
            CommandPrefix = "/",
            ReportChatId = reportChatId,
            EnabledCommandsType = Enums.EnabledCommandsTypes.All,
            DisabledCommands = new []{"rules"}
        });

        var chatMember = _tgChatMemberLogic.Add(new TGChatMember
        {
            ChatId = ubChat.Payload.UBChatId,
            UBUserId = ubUser.Payload.UBUserId,
            UserLevel = Enums.UserLevels.Owner,
            StaffType = Enums.StaffTypes.Platform
        });
        if (chatMember.StatusCode != 0)
        {
            // TODO - Log error
        }

        return tgChat;
    }
        
    public Response<TGChat> Add(TGChat tgChat)
    {
        using var ubc = new UBContext();

        try
        {
            ubc.Add(tgChat);
            ubc.SaveChanges();
            return new Response<TGChat>()
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Payload = tgChat,
                PayloadRaw = tgChat
            };
        }
        catch (Exception ex)
        {
            return new Response<TGChat>()
            {
                StatusCode = 400,
                StatusDescription = ex.Message
            };
        }
    }

    public Response Remove(string ubChatId)
    {
        using var ubc = new UBContext();

        var exists = ubc.TGChats
            .SingleOrDefault(x => x.ChatId == ubChatId);
        if (exists is null)
            return new Response()
            {
                StatusCode = 404,
                StatusDescription = $"TGChat with ChatId {ubChatId} not found"
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

    public Response<TGChat> Update(TGChat tgChat)
    {
        using var ubc = new UBContext();

        var exists = ubc.TGChats
            .SingleOrDefault(x => x.ChatId == tgChat.ChatId);
        if (exists is null)
            return new Response<TGChat>()
            {
                StatusCode = 404,
                StatusDescription = $"TGChat with ChatId {tgChat.ChatId} not found"
            };

        try
        {
            exists.TelegramChatId = tgChat.TelegramChatId;
            exists.Title = tgChat.Title;
            exists.WelcomeText = tgChat.WelcomeText;
            exists.RulesText = tgChat.RulesText;
            exists.SettingsLanguage = tgChat.SettingsLanguage;
            exists.CommandPrefix = tgChat.CommandPrefix;
            exists.ReportChatId = tgChat.ReportChatId;

            ubc.SaveChanges();
            return new Response<TGChat>()
            {
                StatusCode = 200,
                StatusDescription = "OK",
                Payload = tgChat,
                PayloadRaw = tgChat
            };
        }
        catch (Exception ex)
        {
            return new Response<TGChat>()
            {
                StatusCode = 400,
                StatusDescription = ex.Message
            };
        }
    }

    public Response<List<TGChat>> Get()
    {
        using var ubc = new UBContext();
        var tgChats = ubc.TGChats
            .AsNoTracking().ToList();

        return new Response<List<TGChat>>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = tgChats,
            PayloadRaw = tgChats
        };
    }

    public Response<TGChat> Get(string ubChatId)
    {
        using var ubc = new UBContext();
        var tgChat = ubc.TGChats
            .AsNoTracking()
            .SingleOrDefault(x => x.ChatId == ubChatId);

        if (tgChat is null)
            return new Response<TGChat>()
            {
                StatusCode = 404,
                StatusDescription = $"TGChat with ChatId {ubChatId} not found"
            };

        return new Response<TGChat>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = tgChat,
            PayloadRaw = tgChat
        };
    }

    public Response<TGChat> Get(long telegramId)
    {
        using var ubc = new UBContext();
        var tgChat = ubc.TGChats
            .AsNoTracking()
            .SingleOrDefault(x => x.TelegramChatId == telegramId);

        if (tgChat is null)
            return new Response<TGChat>()
            {
                StatusCode = 404,
                StatusDescription = $"TGChat with TelegramChatId {telegramId} not found"
            };

        return new Response<TGChat>()
        {
            StatusCode = 200,
            StatusDescription = "OK",
            Payload = tgChat,
            PayloadRaw = tgChat
        };
    }
}