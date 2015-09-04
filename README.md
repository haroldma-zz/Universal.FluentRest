# Universal.FluentRest

Rest library for consuming apis in a fluent and strong typed manner.

## Instaling

    Install-Package Universal.FluentRest -Pre

## Example

*Request object:*

    /// <summary>
    /// Returns all the IMDb movie reviews for the specified movie.
    /// </summary>
    public class MovieReviewsRequest : RestRequestObject<YtsReviewsResponse>
    {
        public MovieReviewsRequest(uint movieId)
        {
            this.Url("https://yts.to/api/v2/movie_reviews.json").Query("movie_id", movieId);
        }
    }
    
*Utilizing object:*

    var response = await new MovieReviewsRequest(25).ToResponseAsync(); 

## TODO

- [ ] Allow posting XML content.
- [ ] OAuth
