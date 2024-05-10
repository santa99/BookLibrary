namespace View.Model;

public record UpdateBookReqModel(
    int? BookId,
    string Title,
    string Author
);