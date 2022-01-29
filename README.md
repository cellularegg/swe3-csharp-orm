# swe3-csharp-orm


SQL:
```
create table authors
(
    id                   int
        constraint author_pk
            primary key,
    firstname            varchar(255),
    lastname             varchar(255),
    birthdate            timestamp,
    socialsecuritynumber varchar(255),
    salary               double precision
);

create table books
(
    id       int
        constraint book_pk
            primary key,
    title    varchar(255),
    isbn     varchar(255),
    price    double precision,
    authorid int
        constraint book_author_id_fk
            references authors
);

create table stores
(
    Id      integer
        constraint STORES_pk
            primary key,
    Name    VARCHAR(255),
    Address VARCHAR(255)
);
create table BOOKS_IN_STORE
(
    StoreId integer
        constraint BOOKS_IN_STORE_stores_Id_fk
            references stores,
    BookId  integer
        constraint BOOKS_IN_STORE_books_id_fk
            references books,
    constraint BOOKS_IN_STORE_pk
        primary key (BookId, StoreId)
);



```
