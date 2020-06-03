CREATE TABLE [CashPayments] (
    [PaymentId] int NOT NULL IDENTITY,
    [Amount] decimal(18, 2) NOT NULL,
    [Discriminator] nvarchar(max) NOT NULL, #A
    [ReceiptCode] nvarchar(max), #B
    CONSTRAINT [PK_CashPayments] 
		PRIMARY KEY ([PaymentId])
);
#A The Discriminator column holds the name of the class - it is used by EF Core to define what sort of data is saved
#B The ReceiptCode column is only used if it is a PaymentCredit