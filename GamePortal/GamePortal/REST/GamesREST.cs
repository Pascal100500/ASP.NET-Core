using GamePortal.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace GamePortal;

public static class GamesREST
{
    public static void MapGamesApi(this WebApplication app)
    {
        var group = app.MapGroup("/api/games").RequireAuthorization(); // Добавил RequireAuthorization, что бы endpoints внутри этой группы будут доступны только авторизованным пользователям

        group.MapGet("/", async (ApplicationDbContext db) =>
        {
            var games = await db.Games.ToListAsync();
            return Results.Ok(games);
        });

        group.MapGet("/{id}", async (int id, ApplicationDbContext db) =>
        {
            var game = await db.Games.FindAsync(id);

            if (game == null)
                return Results.NotFound($"Игра с ID {id} не найдена.");

            return Results.Ok(game);
        });

        group.MapPost("/", async (CreateGameDto dto, ApplicationDbContext db) =>
        {
            var errors = Validate(dto);
            if (errors.Count > 0)
                return Results.BadRequest(new ValidationProblemDetails(errors));
            var game = new Game
            {
                Title = dto.Title,
                ShortDescription = dto.ShortDescription,
                FullDescription = dto.FullDescription,
                Price = dto.Price,
                AgeLimit = dto.AgeLimit,
                ImageUrl = dto.ImageUrl,
                Screenshot1 = dto.Screenshot1,
                Screenshot2 = dto.Screenshot2,
                Screenshot3 = dto.Screenshot3,
                IsOnSale = dto.IsOnSale,
                IsTopGame = dto.IsTopGame,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            db.Games.Add(game);
            await db.SaveChangesAsync();

            return Results.Created($"/api/games/{game.Id}", game);
        })
        .RequireAuthorization("AdminOnly");

        group.MapPut("/{id}", async (int id, UpdateGameDto dto, ApplicationDbContext db) =>
        {
            var errors = Validate(dto);
            if (errors.Count > 0)
                return Results.BadRequest(new ValidationProblemDetails(errors));

            var game = await db.Games.FindAsync(id);
            if (game == null)
                return Results.NotFound($"Игра с ID {id} не найдена.");

            game.Title = dto.Title;
            game.ShortDescription = dto.ShortDescription;
            game.FullDescription = dto.FullDescription;
            game.Price = dto.Price;
            game.AgeLimit = dto.AgeLimit;
            game.ImageUrl = dto.ImageUrl;
            game.Screenshot1 = dto.Screenshot1;
            game.Screenshot2 = dto.Screenshot2;
            game.Screenshot3 = dto.Screenshot3;
            game.IsOnSale = dto.IsOnSale;
            game.IsTopGame = dto.IsTopGame;
            game.IsActive = dto.IsActive;

            await db.SaveChangesAsync();

            return Results.Ok(game);
        })
        .RequireAuthorization("AdminOnly");

        group.MapDelete("/{id}", async (int id, ApplicationDbContext db) =>
        {
            var game = await db.Games.FindAsync(id);

            if (game == null)
                return Results.NotFound($"Игра с ID {id} не найдена.");

            db.Games.Remove(game);
            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .RequireAuthorization("AdminOnly");

    }

    public class CreateGameDto
    {
        [Required(ErrorMessage = "Название обязательно")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Краткое описание обязательно")]
        public string ShortDescription { get; set; } = "";

        [Required(ErrorMessage = "Полное описание обязательно")]
        public string FullDescription { get; set; } = "";

        [Range(0, 100000, ErrorMessage = "Цена должна быть больше или равна 0")]
        public decimal Price { get; set; }

        [Range(3, 18, ErrorMessage = "Возраст от 3 до 18")]
        public int AgeLimit { get; set; }

        public string? ImageUrl { get; set; } = "";

        public string? Screenshot1 { get; set; }
        public string? Screenshot2 { get; set; }
        public string? Screenshot3 { get; set; }

        public bool IsOnSale { get; set; }
        public bool IsTopGame { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

    }

    // Хелпер валидации, интегрируемый в любой endpoint by Kartuzov Aleksandr
    private static Dictionary<string, string[]> Validate(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, context, results, true);

        var errors = new Dictionary<string, string[]>();
        foreach (var result in results)
        {
            foreach (var memberName in result.MemberNames)
            {
                if (!errors.ContainsKey(memberName))
                    errors[memberName] = new string[] { result.ErrorMessage ?? "" };
                else
                    errors[memberName] = errors[memberName].Append(result.ErrorMessage ?? "").ToArray();
            }
        }
        return errors;
    }
    public class UpdateGameDto
    {
        [Required(ErrorMessage = "Название обязательно")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Краткое описание обязательно")]
        public string ShortDescription { get; set; } 

        [Required(ErrorMessage = "Полное описание обязательно")]
        public string FullDescription { get; set; } 

        [Range(0, 100000, ErrorMessage = "Цена должна быть больше или равна 0")]
        public decimal Price { get; set; }

        [Range(3, 18, ErrorMessage = "Возраст от 3 до 18")]
        public int AgeLimit { get; set; }

        public string? ImageUrl { get; set; }

        public string? Screenshot1 { get; set; }
        public string? Screenshot2 { get; set; }
        public string? Screenshot3 { get; set; }

        public bool IsOnSale { get; set; }
        public bool IsTopGame { get; set; }
        public bool IsActive { get; set; }
    }
}
