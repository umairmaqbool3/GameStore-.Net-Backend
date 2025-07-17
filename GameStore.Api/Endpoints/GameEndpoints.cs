using System;
using GameStore.Api.Dtos;

namespace GameStore.Api.Endpoints;

public static class GameEndpoints
{
    const string GetGameEndpointName = "GetGame";

    private static readonly List<GameDto> games =
    [
        new(
            1,
            "Street Fighter II",
            "Fighting",
            19.99M,
            new DateOnly(1992, 7, 15)
        ),
        new(
            2,
            "Super Mario World",
            "Platformer",
            29.99M,
            new DateOnly(1990, 11, 21)
        ),
        new(
            3,
            "Final Fantasy VII",
            "Roleplaying",
            59.99M,
            new DateOnly(2010, 9, 30)
        )
    ];

    public static RouteGroupBuilder MapGamesEndPoints(this WebApplication app)
    {
        var group = app.MapGroup("games").WithParameterValidation();

        group.MapGet("/", () => games);

        group.MapGet("/{id}", (int id) =>
        {
            GameDto? game = games.Find(game => game.Id == id);
            return game is null ? Results.NotFound() : Results.Ok(game);
        })
            .WithName(GetGameEndpointName);

        group.MapPost("/", (CreateGameDto gameDto) =>
        {
            var newGame = new GameDto(
                games.Count + 1,
                gameDto.Name,
                gameDto.Genre,
                gameDto.Price,
                gameDto.ReleaseDate
            );
            games.Add(newGame);
            return Results.CreatedAtRoute(GetGameEndpointName, new { id = newGame.Id }, newGame);
        });

        group.MapPut("/{id}", (int id, UpdateGameDto gameDto) =>
        {
            var index = games.FindIndex(game => game.Id == id);
            if (index == -1)
            {
                return Results.NotFound();
            }
            games[index] = new GameDto(
                id,
                gameDto.Name,
                gameDto.Genre,
                gameDto.Price,
                gameDto.ReleaseDate
            );

            return Results.NoContent();
        });

        group.MapDelete("/{id}", (int id) =>
        {
            var index = games.FindIndex(game => game.Id == id);
            if (index == -1)
            {
                return Results.NotFound();
            }

            games.RemoveAt(index);
            return Results.NoContent();
        });

        return group;
    }
}
