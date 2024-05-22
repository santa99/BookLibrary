namespace View.Model;

public record CreateBorrowReqModel(
    int BookId,
    int ReadersCardId,
    DateTimeOffset From
);