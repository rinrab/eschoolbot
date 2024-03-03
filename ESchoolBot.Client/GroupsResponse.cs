namespace ESchoolBot.Client
{
    public class GroupsResponse : List<GroupsResponse.Group>
    {
        public class Group
        {
            public int GroupId { get; set; }
        }
    }

    public class PeriodsResponse
    {
        public required Period[] Items { get; set; }

        public class Period
        {
            public required int Id { get; set; }
        }
    }
}