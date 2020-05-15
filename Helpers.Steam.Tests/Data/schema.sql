CREATE SCHEMA `steam` ;

CREATE TABLE `steam`.`user` (
  `id` BIGINT NOT NULL,
  `name` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`id`));

CREATE TABLE `steam`.`app` (
  `id` INT NOT NULL,
  `name` VARCHAR(100) NOT NULL,
  PRIMARY KEY (`id`));

CREATE TABLE `steam`.`userapp` (
  `userid` BIGINT NOT NULL,
  `appid` INT NOT NULL,
  `hours` SMALLINT NOT NULL,
  PRIMARY KEY (`userid`, `appid`),
  CONSTRAINT `fk_userapp_userid_user_id`
    FOREIGN KEY (`userid`)
    REFERENCES `steam`.`user` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `fk_userapp_appid_app_id`
    FOREIGN KEY (`appid`)
    REFERENCES `steam`.`app` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);

CREATE TABLE `steam`.`category` (
  `id` SMALLINT NOT NULL,
  `name` VARCHAR(100) NOT NULL,
  PRIMARY KEY (`id`));

CREATE TABLE `steam`.`appcategory` (
  `appid` INT NOT NULL,
  `categoryid` SMALLINT NOT NULL,
  PRIMARY KEY (`appid`, `categoryid`),
  CONSTRAINT `fk_appcategory_appid_app_id`
    FOREIGN KEY (`appid`)
    REFERENCES `steam`.`app` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `fk_appcategory_categoryid_category_id`
    FOREIGN KEY (`categoryid`)
    REFERENCES `steam`.`category` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);