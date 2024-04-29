﻿using Contracts;

namespace Api;

public class BorrowBookCommand : IBorrowBookCommand
{
    private readonly IBookLibraryRepository _bookLibraryRepository;

    public BorrowBookCommand(
        IBookLibraryRepository bookLibraryRepository)
    {
        _bookLibraryRepository = bookLibraryRepository;
    }

    public Task<BorrowModel> BorrowBook(int bookId, int readersCardId, DateTimeOffset borrowed,
        CancellationToken cancellationToken)
    {
        return _bookLibraryRepository.BorrowBook(bookId, readersCardId, borrowed, cancellationToken);
    }

    public Task<int> ReturnBook(int bookId, CancellationToken cancellationToken)
    {
        return _bookLibraryRepository.ReturnBook(bookId, cancellationToken);
    }
}