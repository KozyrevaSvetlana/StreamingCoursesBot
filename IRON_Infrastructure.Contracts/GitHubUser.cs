using Newtonsoft.Json;

namespace StreamingCourses_Contracts
{
    public class GitHubUser
    {
        [JsonProperty("login")]
        public string Login { get; set; } = default!;

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("node_id")]
        public string NodeId { get; set; } = default!;

        [JsonProperty("avatar_url")]
        public Uri AvatarUrl { get; set; } = default!;

        [JsonProperty("gravatar_id")]
        public string GravatarId { get; set; } = default!;

        [JsonProperty("url")]
        public Uri Url { get; set; } = default!;

        [JsonProperty("html_url")]
        public Uri HtmlUrl { get; set; } = default!;

        [JsonProperty("followers_url")]
        public Uri FollowersUrl { get; set; } = default!;

        [JsonProperty("following_url")]
        public string FollowingUrl { get; set; } = default!;

        [JsonProperty("gists_url")]
        public string GistsUrl { get; set; } = default!;

        [JsonProperty("starred_url")]
        public string StarredUrl { get; set; } = default!;

        [JsonProperty("subscriptions_url")]
        public Uri SubscriptionsUrl { get; set; } = default!;

        [JsonProperty("organizations_url")]
        public Uri OrganizationsUrl { get; set; } = default!;

        [JsonProperty("repos_url")]
        public Uri ReposUrl { get; set; } = default!;

        [JsonProperty("events_url")]
        public string EventsUrl { get; set; } = default!;

        [JsonProperty("received_events_url")]
        public Uri ReceivedEventsUrl { get; set; } = default!;

        [JsonProperty("type")]
        public string Type { get; set; } = default!;

        [JsonProperty("site_admin")]
        public bool SiteAdmin { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = default!;

        [JsonProperty("company")]
        public object Company { get; set; } = default!;

        [JsonProperty("blog")]
        public string Blog { get; set; } = default!;

        [JsonProperty("location")]
        public string Location { get; set; } = default!;

        [JsonProperty("email")]
        public object Email { get; set; } = default!;

        [JsonProperty("hireable")]
        public object Hireable { get; set; } = default!;

        [JsonProperty("bio")]
        public string Bio { get; set; } = default!;

        [JsonProperty("twitter_username")]
        public object TwitterUsername { get; set; } = default!;

        [JsonProperty("public_repos")]
        public long PublicRepos { get; set; }

        [JsonProperty("public_gists")]
        public long PublicGists { get; set; }

        [JsonProperty("followers")]
        public long Followers { get; set; }

        [JsonProperty("following")]
        public long Following { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
