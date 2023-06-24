namespace UserManagement.Models.Users;

public class UserNotFoundViewModel
{
    public UserNotFoundViewModel(long id)
    {
        Id = id;
    }
    
    public long Id { get; set; }
}