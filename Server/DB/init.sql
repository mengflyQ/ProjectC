drop table if exists `UserInfo`;
create table `UserInfo`(
    `Account` varchar(20) not null,
	`Password` varchar(32) not null,
	`UserId` int unsigned not null auto_increment,
	`Token` varchar(32) not null,
	`RegisterTime` timestamp not null default CURRENT_TIMESTAMP,
	`LastLoginTime` timestamp not null default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,
	primary key(`Account`),
	key user_id (`UserId`)
)engine=innodb default charset=utf8 auto_increment=1;

insert into `UserInfo` values ('user0', 'a123456', 1, '000', CURRENT_TIMESTAMP, CURRENT_TIMESTAMPuserinfo);userinfo