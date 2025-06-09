Online Payments Site
A website for online payments of a financial institution needs to be implemented. In it, users should be able to log in, check the available amounts on their accounts, make payments and see a report on the ordered payments.
Object model
Accounts
Each account has:
 a unique identifier
 an account number
 an available amount on the account
Users
Each user has:
 a unique identifier
 usernames
 a username (which is used for login)
 a password
User-account relationships
One account is allowed to be subscribed to more than one user. There cannot be accounts
that are not subscribed to any user.
A user can be subscribed to many accounts, but cannot be subscribed to one account
more than once (this uniqueness must be ensured). There can be a user without accounts.
For the purposes of the task, the accounts, users and the relationships between them are entered manually by the developer into the database (i.e., there is no need to develop tools and functionality for registering and managing accounts and clients).
Payments
The payment attributes are:
 Account from which the payment was made
 Account to which the payment was made
 Amount - a number with 2 decimal places
 Reason for the transfer - a string with a maximum length of 32 characters



Application functionalities to be developed
User login
Login to the application, after successful verification of the username/password for the respective
client.
Show list of accounts
Visualization of the main page with a list of user accounts with the available amount on each account
Creating a new payment
A form in which all attributes are mandatory for selection (entry):
 Account from which to make the payment – ​​a list of accounts to which the user is subscribed must be visualized with information "number – available amount" and the possibility of selecting from this list
 Account to which to make the payment – ​​field with validation exactly 22 characters, only digits and Latin letters
 Amount – a number with two decimal places
 Reason – a string with a maximum length of 32 characters
 When created, the payment is saved in the status WAITING
Displaying a list of payments
All payments saved by the user with their attributes and current status are visualized, with the possibility of sorting by two different criteria – chronologically (from the newest to the oldest) and by status (those with status PENDING at the top)
Sending a payment
The operation is possible only if there is sufficient available funds in the account to make the payment
The payment must be in the PENDING status
After successful sending, its status becomes PROCESSED and the available amount on the account from which the payment is made decreases accordingly
Rejecting a payment
The payment must be in the PENDING status
After rejection, its status becomes CANCELED

Implementation requirements and guidelines

The method of implementation, the interpretation of functional requirements, etc. are entirely at the discretion and understanding of the developer.
The software architecture, database design and user interface must be defined and implemented by the developer with a demonstration of good practices for software development - 3-tier architecture, appropriate object model, database design with the necessary constraints, etc. The recommended UI technology is MVC.
The task is intended to implement the main functionalities in a decent working condition in about 1 full working day for independent work by one developer.
Additionally (outside the exam time of 8 hours), after submitting the work, the developer must also compile a short list of the functionalities that he did not implement,
possible improvements or extensions that he would like to complete, but was unable to due to the
limited time of one working day.