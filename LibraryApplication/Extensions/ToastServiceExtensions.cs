using Microsoft.FluentUI.AspNetCore.Components;

namespace LibraryApplication.Extensions;

public static class ToastServiceExtensions
{
    const int TimeoutMs = 5000;

    extension(IToastService toastService)
    {
        public void Error(string message) =>
            toastService.ShowError(message, TimeoutMs);

        public void Success(string message) =>
            toastService.ShowSuccess(message, TimeoutMs);

        public void Info(string message) =>
            toastService.ShowInfo(message, TimeoutMs);

        public void InvalidUser() =>
            toastService.Error(ErrorMessages.InvalidUser);

        public void BookNotFound() =>
            toastService.Error(ErrorMessages.BookNotFound);

        public void UserNotFound() =>
            toastService.Error(ErrorMessages.UserNotFound);

        public void FailedToBorrowBook() =>
            toastService.Error(ErrorMessages.FailedToBorrowBook);

        public void FailedToReturnBook() =>
            toastService.Error(ErrorMessages.FailedToReturnBook);

        public void FailedToAddBook() =>
            toastService.Error(ErrorMessages.FailedToAddBook);

        public void FailedToAddBookCopy() =>
            toastService.Error(ErrorMessages.FailedToAddBookCopy);

        public void FailedToRemoveBook() =>
            toastService.Error(ErrorMessages.FailedToRemoveBook);

        public void FailedToRemoveBookCopy() =>
            toastService.Error(ErrorMessages.FailedToRemoveBookCopy);

        public void BookCopyAdded() =>
            toastService.Info(InfoMessages.BookCopyAdded);

        public void BookCopyRemoved() =>
            toastService.Info(InfoMessages.BookCopyRemoved);

        public void BookAdded() =>
            toastService.Info(InfoMessages.BookAdded);

        public void BookRemoved() =>
            toastService.Info(InfoMessages.BookRemoved);

        public void BookBorrowed() =>
            toastService.Info(InfoMessages.BookBorrowed);

        public void BookReturned() =>
            toastService.Info(InfoMessages.BookReturned);
    }
}

static class ErrorMessages
{
    public const string InvalidUser = "Invalid User";
    public const string BookNotFound = "Book not found";
    public const string UserNotFound = "User not found";
    public const string FailedToAddBook = "Failed to add book";
    public const string FailedToRemoveBook = "Failed to remove book copy";
    public const string FailedToAddBookCopy = "Failed to add book copy";
    public const string FailedToRemoveBookCopy = "Failed to remove book copy";
    public const string FailedToBorrowBook = "Failed to borrow book";
    public const string FailedToReturnBook = "Failed to return book";
}

static class InfoMessages
{
    public const string BookAdded = "Book added";
    public const string BookRemoved = "Book removed";
    public const string BookCopyAdded = "Book copy added";
    public const string BookCopyRemoved = "Book copy removed";
    public const string BookBorrowed = "Book borrowed";
    public const string BookReturned = "Book returned";
}