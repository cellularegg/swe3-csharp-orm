# swe3-csharp-orm


SQL:
```
create table TEACHERS
(
    ID        VARCHAR(24) not null
        primary key,
    NAME      TEXT,
    FIRSTNAME TEXT,
    GENDER    INTEGER,
    BDATE     TIMESTAMP,
    HDATE     TIMESTAMP,
    SALARY    INTEGER
);
create table CLASSES
(
    ID       VARCHAR(24) not null
        primary key,
    NAME     TEXT,
    KTEACHER VARCHAR(24) not null
        references TEACHERS
);
create table STUDENTS
(
    ID        VARCHAR(24) not null
        primary key,
    NAME      TEXT,
    FIRSTNAME TEXT,
    GENDER    INTEGER,
    BDATE     TIMESTAMP,
    KCLASS    VARCHAR(24)
        references CLASSES,
    GRADE     INTEGER
);
create table COURSES
(
    ID       VARCHAR(24) not null
        primary key,
    HACTIVE  INTEGER default 0 not null,
    NAME     TEXT,
    KTEACHER VARCHAR(24) not null
        references TEACHERS
);
create table STUDENT_COURSES
(
    KSTUDENT VARCHAR(24) not null
        references STUDENTS,
    KCOURSE  VARCHAR(24) not null
        references COURSES
);




```
