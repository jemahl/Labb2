using PlacementService.Ui.Models;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;

namespace PlacementService.Ui.Services;

public sealed class PlacementApiClient
{
    private readonly HttpClient _httpClient;

    public PlacementApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResult<PlacementSearchResponse>> SearchAsync(
        string query,
        string? region,
        int limit,
        int offset,
        CancellationToken cancellationToken)
    {
        try
        {
            // Step 1: build a dictionary with query, limit and offset, plus region if provided 
            // to act as the parameters for the API-query. Build a url variable to include the 
            // API endpoint and the query parameters
            // HINT: Use QueryHelpers.AddQueryString to append parameters to the base path
            var queryParams = new Dictionary<string, string?>
            {
                ["q"] = query,
                ["limit"] = limit.ToString(),
                ["offset"] = offset.ToString(),
            };
            if (!string.IsNullOrWhiteSpace(region))
            {
                queryParams["region"] = region;
            }

            //Skapar en url med parametrarna från min dictionary som skickar till search api
            var relativeUrl = QueryHelpers.AddQueryString("/api/placements/search", queryParams);

            // Step 2: call _httpClient.GetAsync() with the full URL and inspect the 
            // response.StatusCode. You should then handle the following responses correctly:
            //   - 200 OK: deserialize JSON into PlacementSearchResponse.
            // HINT: In case of a successful response, ReadFromJsonAsync can be used on 
            // the response´s Content property to read the data.
            //   - 400 BadRequest: return a friendly message like "Felaktiga parametrar".
            //   - 404 NotFound: return "Inga resultat hittades".
            //   - Other status codes: return a generic error.

            //skickar min url förfrågan
            var response = await _httpClient.GetAsync(relativeUrl, cancellationToken);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    var result = await response.Content.ReadFromJsonAsync<PlacementSearchResponse>(cancellationToken);

                    if (result == null)
                    {
                        return new ApiResult<PlacementSearchResponse>(null, "Ogiltigt svar från API");
                    }

                    return new ApiResult<PlacementSearchResponse>(result, null);
                }
                catch
                {
                    return new ApiResult<PlacementSearchResponse>(null, "Fel uppstod hos API");
                }

            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return new ApiResult<PlacementSearchResponse>(null, "Felaktiga parametrar");
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new ApiResult<PlacementSearchResponse>(null, "Inga resultat hittades");
            }
            if (!response.IsSuccessStatusCode)
            {
                return new ApiResult<PlacementSearchResponse>(null, "Error");
            }
            // Step 3: return a ApiResult<PlacementSearchResponse> with data or an error.
        }
        catch
        {
            return new ApiResult<PlacementSearchResponse>(null, "Kunde inte nå PlacementServiceAPI");
        }
        // TODO: Replace the exception and return a friendly error if the PlacementServiceAPI cannot be reached.
        return new ApiResult<PlacementSearchResponse>(null, "Error");
    }

    // offset is included for API consistency with SearchAsync but summary 
    // always uses offset=0 so the grouping reflects the full result set - not a 
    // single page.
    public async Task<ApiResult<PlacementSummaryResponse>> SummaryAsync(
        string query,
        string? region,
        int limit,
        int offset,
        CancellationToken cancellationToken)
    {
        try
        {
            var queryParams = new Dictionary<string, string?>
            {
                ["q"] = query,
                ["limit"] = limit.ToString(),
                ["offset"] = offset.ToString(),
            };
            if (!string.IsNullOrWhiteSpace(region))
            {
                queryParams["region"] = region;
            }

            var relativeUrl = QueryHelpers.AddQueryString("/api/placements/summary", queryParams);
            var response = await _httpClient.GetAsync(relativeUrl, cancellationToken);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    var result = await response.Content.ReadFromJsonAsync<PlacementSummaryResponse>(cancellationToken);

                    if (result == null)
                    {
                        return new ApiResult<PlacementSummaryResponse>(null, "Ogiltigt svar från API");
                    }

                    return new ApiResult<PlacementSummaryResponse>(result, null);
                }
                catch
                {
                    return new ApiResult<PlacementSummaryResponse>(null, "Fel uppstod hos API");
                }
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return new ApiResult<PlacementSummaryResponse>(null, "Felaktiga parametrar");
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new ApiResult<PlacementSummaryResponse>(null, "Inga resultat hittades");
            }
            if (!response.IsSuccessStatusCode)
            {
                return new ApiResult<PlacementSummaryResponse>(null, "Error");
            }
        }
        catch
        {
            return new ApiResult<PlacementSummaryResponse>(null, $"Kunde inte nå PlacementServiceAPI");
        }
        return new ApiResult<PlacementSummaryResponse>(null, "Error");

        // TODO: Follow the same pattern as in SearchAsync but call the summary endpoint instead

        // TODO: Replace the exception and return a ApiResult<PlacementSummaryResponse> with data or with an error.
    }
}
