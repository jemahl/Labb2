using PlacementService.Ui.Models;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using System.Net.Http.Json;

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
            var queryParams = new Dictionary<string, string?>
            {
                ["q"] = query,
                ["limit"] = limit.ToString(),
                ["offset"] = offset.ToString()
            };
            if (region != null)
            {
                queryParams["region"] = region;
            }

            var relativeUrl = QueryHelpers.AddQueryString("search", queryParams);
            // Step 1: build a dictionary with query, limit and offset, plus region if provided to act as the parameters for the API-query. Build a url variable to include the API endpoint and the query parameters
            // HINT: Use QueryHelpers.AddQueryString to append parameters to the base path


            using var response = await _httpClient.GetAsync(relativeUrl);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = await response.Content.ReadFromJsonAsync<PlacementSearchResponse>(cancellationToken);
                return new ApiResult<PlacementSearchResponse>(result, null);

            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return new ApiResult<PlacementSearchResponse>(null, "Felaktiga parametrar");
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new ApiResult<PlacementSearchResponse>(null, "Inga resultat hittades");
            }
            else
            {
                return new ApiResult<PlacementSearchResponse>(null, "Error");
            }
        }
        catch
        {
            return new ApiResult<PlacementSearchResponse>(null, "Error");
        }
        //return new ApiResult<PlacementSearchResponse>(null, "Error");
        // Step 2: call _httpClient.GetAsync() with the full URL and inspect the response.StatusCode. You should then handle the following responses correctly:
        //   - 200 OK: deserialize JSON into PlacementSearchResponse.
        // HINT: In case of a successful response, ReadFromJsonAsync can be used on the response´s Content property to read the data.
        //   - 400 BadRequest: return a friendly message like "Felaktiga parametrar".
        //   - 404 NotFound: return "Inga resultat hittades".
        //   - Other status codes: return a generic error.

        // Step 3: return a ApiResult<PlacementSearchResponse> with data or an error.

        // TODO: Replace the exception and return a friendly error if the PlacementServiceAPI cannot be reached.
    }

    // offset is included for API consistency with SearchAsync but summary always uses offset=0 so the grouping reflects the full result set - not a single page.
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
                ["offset"] = offset.ToString()
            };
            if (region != null)
            {
                queryParams["region"] = region;
            }

            var relativeUrl = QueryHelpers.AddQueryString("summary", queryParams);
            // Step 1: build a dictionary with query, limit and offset, plus region if provided to act as the parameters for the API-query. Build a url variable to include the API endpoint and the query parameters
            // HINT: Use QueryHelpers.AddQueryString to append parameters to the base path


            using var response = await _httpClient.GetAsync(relativeUrl);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = await response.Content.ReadFromJsonAsync<PlacementSummaryResponse>(cancellationToken);
                return new ApiResult<PlacementSummaryResponse>(result, null);

            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return new ApiResult<PlacementSummaryResponse>(null, "Felaktiga parametrar");
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new ApiResult<PlacementSummaryResponse>(null, "Inga resultat hittades");
            }
            else
            {
                return new ApiResult<PlacementSummaryResponse>(null, "Error");
            }
        }
        catch
        {
            return new ApiResult<PlacementSummaryResponse>(null, "Error");
        }
        // TODO: Follow the same pattern as in SearchAsync but call the summary endpoint instead

        // TODO: Replace the exception and return a ApiResult<PlacementSummaryResponse> with data or with an error.
    }
}
