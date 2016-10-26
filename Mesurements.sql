CREATE TABLE `Measurements` (
`Id`    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    `RefuelingDate`    DATE NOT NULL,
    `Counter`    INTEGER NOT NULL,
    `Amount`    NUMERIC NOT NULL,
    `Price`    NUMERIC NOT NULL
);