﻿drop table Creatures
go
create table Creatures
(
	TableKey int Identity(1,1) not null,
	RunTime numeric(9,4) not null,
	GameSeed int not null,
	SessionID int not null,
	MapSize int not null,
	CreatureRatio numeric(10,10) not null,
	FoodRatio numeric(10,10) not null,
	IsAlive varchar(10) not null,
	DeathCause varchar(200) not null,
	CreatureID int not null,
	CreatureName varchar(200) not null,
	SpeciesID int not null,
	SpeciesName varchar(200) not null,
	SpeciesStrain varchar(1000) not null,
	OriginalSpeciesID int not null,
	OriginalSpeciesName varchar(200) not null,
	Generation int not null,
	FoodType varchar(10) not null,
	FoodDigestionRate numeric(9,4) not null,
	EggInterval numeric(9,4) not null,
	EggIncubation numeric(9,4) not null,
	EggIncubationActual numeric(9,4) not null,
	EggCamo numeric(9,4) not null,
	EggToxicity numeric(9,4) not null,
	EggsCreatedTotal int not null,
	Speed numeric(9,4) not null,
	LifeSpan numeric(9,4) not null,
	Sight numeric(9,4) not null,
	Attraction numeric(9,4) not null,
	Camo numeric(9,4) not null,
	Cloning numeric(9,4) not null,
	Herbavore numeric(9,4) not null,
	Carnivore numeric(9,4) not null,
	Omnivore numeric(9,4) not null,
	Scavenger numeric(9,4) not null,
	ColdClimateTolerance numeric(9,4) not null,
	HotClimateTolerance numeric(9,4) not null
)