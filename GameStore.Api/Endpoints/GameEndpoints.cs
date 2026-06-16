using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;
//ing system.ComponentModel.DataAnnotations;
namespace GameStore.Api.Endpoints;

public static class GameEndpoints
{
   
  public static RouteGroupBuilder MapGameEndpoints(this WebApplication app)
  { 
    var group=app.MapGroup("games");
       // GET /games
     group.MapGet("/",async(GameStoreContext dbContext)=> 
       await dbContext.Games
          .Include(game => game.Genre)
          .Select(game => game.ToGameSummaryDto())
          .AsNoTracking()
          .ToListAsync());

      // GET /games/1
     group.MapGet("/{id}", async (int id, GameStoreContext dbContext) => 
    {
     Game?game= await dbContext.Games.FindAsync(id);
     if(game==null)
     return Results.NotFound();
     else
     return Results.Ok(game.ToGameDetailsDto());
    })
   .WithName("GetGame");

  // POST /games
  group.MapPost("/",async (CreateGameDto newGame, GameStoreContext dbContext) =>  
  {
     Game game = newGame.ToEntity();
     game.Genre = await dbContext.Genres.FindAsync(newGame.GenreId);

     dbContext.Games.Add(game);
     await dbContext.SaveChangesAsync();  


     return Results.CreatedAtRoute("GetGame", new{id=game.Id},game.ToGameSummaryDto());
    });

     // PUT /games
     group.MapPut("/{id}",async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext)=>
    {
       var existingGame = await dbContext.Games.FindAsync(id);
         if(existingGame==null)
         {
            return Results.NotFound();
         }
   
        dbContext.Entry(existingGame).CurrentValues.SetValues(updatedGame.ToEntity(id));
        await dbContext.SaveChangesAsync();
      
          return Results.NoContent();
      });

     // delete /games
     group.MapDelete("/{id}",async (int id, GameStoreContext dbContext) =>
     {
       await dbContext.Games
                      .Where(game => game.Id == id)
                      .ExecuteDeleteAsync();

     return Results.NoContent();
    });

    return group;
 }
}
    