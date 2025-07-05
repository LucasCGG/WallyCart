namespace WallyCart.Enums;

public enum RemoveUserResult
{
    Success,
    NotFound,
    NotAdmin,
    IsLastAdmin,
    TransferredAdmin,
    GroupDeleted
}