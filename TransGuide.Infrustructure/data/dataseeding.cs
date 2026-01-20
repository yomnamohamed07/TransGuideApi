
using System.Text.Json;
using TransGuide.Data;
using TransGuide.Data.Entities.ApplicationEntities;

namespace TransGuide.Infrustructure.data
{
	public static class dataseeding
	{
		public async static Task SeedAsync(TransGuideDbContext transGuideDbContext)
		{
			if (!transGuideDbContext.Routes.Any())
			{
				var route = File.ReadAllText("../TransGuide.Infrustructure/data/dataseeding/Route.json");
				var routes = JsonSerializer.Deserialize<List<Route>>(route);
				if (routes?.Count > 0)
				{
					foreach (var Route in routes)
					{
						await transGuideDbContext.Routes.AddAsync(Route);
					}
					await transGuideDbContext.SaveChangesAsync();
				}
			}
			if (!transGuideDbContext.Stations.Any())
			{
				var station = File.ReadAllText("../TransGuide.Infrustructure/data/dataseeding/Station.json");
				var stations = JsonSerializer.Deserialize<List<Station>>(station);
				if (stations?.Count > 0)
				{
					foreach (var Station in stations)
					{
						await transGuideDbContext.Stations.AddAsync(Station);
					}
					await transGuideDbContext.SaveChangesAsync();

				}
			}
		
			if (!transGuideDbContext.RouteStations.Any())
			{
				var routestation = File.ReadAllText("../TransGuide.Infrustructure/data/dataseeding/RouteStation.json");
				var routestations = JsonSerializer.Deserialize<List<RouteStation>>(routestation);
				if (routestations?.Count > 0)
				{
					foreach (var RouteStation in routestations)
					{
						await transGuideDbContext.RouteStations.AddAsync(RouteStation);
					}
					await transGuideDbContext.SaveChangesAsync();
				}
			}

		}



	}
}


