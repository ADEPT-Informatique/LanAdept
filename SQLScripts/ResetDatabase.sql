USE LanAdept;
GO



/* "Nettoie" la base de donn�e */

EXEC sp_MSForEachTable 'IF OBJECT_ID(''?'') NOT IN (ISNULL(OBJECT_ID(''[dbo].[__MigrationHistory]''),0))
						ALTER TABLE ? NOCHECK CONSTRAINT ALL'
GO
EXEC sp_MSForEachTable 'IF OBJECT_ID(''?'') NOT IN (ISNULL(OBJECT_ID(''[dbo].[__MigrationHistory]''),0))
						DELETE FROM ?'
GO
EXEC sp_MSForEachTable 'IF OBJECT_ID(''?'') NOT IN (ISNULL(OBJECT_ID(''[dbo].[__MigrationHistory]''),0))
						DBCC CHECKIDENT(''?'', RESEED, 0)'
GO
EXEC sp_MSForEachTable 'IF OBJECT_ID(''?'') NOT IN (ISNULL(OBJECT_ID(''[dbo].[__MigrationHistory]''),0))
						ALTER TABLE ? CHECK CONSTRAINT ALL'
GO

IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_Permissions_Name') 
    DROP INDEX IX_Permissions_Name ON [dbo].[Permissions]; 
GO
CREATE NONCLUSTERED INDEX IX_Permissions_Name
    ON [dbo].[Permissions] (Name); 
GO


/* ============================================== *
 *				Insertion des r�les				  *
 *=============================================== */
 DECLARE @guest			int = 0;
 DECLARE @Unconfirmed	int = 0;
 DECLARE @User			int = 10;
 DECLARE @AdminTourn	int = 110;
 DECLARE @AdminPlace	int = 120;
 DECLARE @AdminFull		int = 200;
 DECLARE @AdminSuper	int = 9001;

INSERT INTO [dbo].[Roles] (Name, PermissionLevel, IsReadOnly, IsDefaultRole, IsOwnerRole, IsUnconfirmedRole)
	VALUES   ('Non confirm�',@Unconfirmed,1,0,0,1)
			,('Utilisateur',@User,1,1,0,0)
			,('Admin tournoi',@AdminTourn,0,0,0,0)
			,('Admin accueil',@AdminPlace,0,0,0,0)
			,('Full admin',@AdminFull,0,0,0,0)
			,('Super admin',@AdminSuper,1,0,1,0)

/* ============================================== *
 *			Insertion des permissions			  *
 *=============================================== */

INSERT INTO [dbo].[Permissions] (Name, Description, MinimumRoleLevel, IsReadOnly)
	VALUES	 
			('all.logout', 'Un utilisateur peut se d�connecter', @guest, 1)

			/* Section des permissions pour le site user side */
			,('user.home.index', 'Afficher la page d''accueil du LAN', @guest, 0)
			,('user.home.about', 'Afficher la page � propos', @guest, 0)

			,('user.place.index', 'Afficher la liste des places', @guest, 0)
			,('user.place.reserver', 'R�server une place', @User, 0)
			,('user.place.maPlace', 'Afficher sa r�servation', @User, 0)
			,('user.place.cancel', 'Annuler sa r�servation', @User, 0)
			,('user.place.getBarcode', 'Afficher un code barre', @User, 0)

			,('user.tournament.index', 'Afficher la liste des tournois', @guest, 0)
			,('user.tournament.details', 'Afficher les d�tails d''un tournoi', @guest, 0)
			,('user.tournament.gamertag.add', 'Ajouter un gamertag', @User, 0)
			,('user.tournament.team.details', 'Afficher les d�tails d''une �quipe', @User, 0)
			,('user.tournament.team.add', 'Ajouter une �quipe', @User, 0)
			,('user.tournament.team.join', 'Rejoindre une �quipe', @User, 0)
			,('user.tournament.team.kick', 'Exclure un membre d''une �quipe', @User, 0)

			/* Section des permissions pour le site admin side */
			,('admin.login', 'Connexion au panneau d''admin', @AdminTourn, 0)

			,('admin.game.index', 'Afficher la liste des jeux', @AdminTourn, 0)
			,('admin.game.details', 'Afficher les d�tails d''un jeu', @AdminTourn, 0)
			,('admin.game.create', 'Ajouter un jeu', @AdminTourn, 0)
			,('admin.game.edit', 'Modifier un jeu', @AdminTourn, 0)
			,('admin.game.delete', 'Supprimer un jeu', @AdminTourn, 0)

			,('admin.general.settings', 'Changer les param�tres g�n�raux du LAN', @AdminFull, 0)
			,('admin.general.rules', 'Modifier les r�glements du LAN', @AdminFull, 0)
			,('admin.general.description', 'Modifier la description du LAN', @AdminFull, 0)
			,('admin.general.rememberEmail', 'Modifier le message de rappel du LAN', @AdminFull, 0)

			,('admin.home.index', 'Afficher la page d''accueil du panneau d''admin', @AdminTourn, 0)

			,('admin.place.list', 'Afficher la liste des places', @AdminTourn, 0)
			,('admin.place.details', 'Afficher les d�tails d''une place', @AdminTourn, 0)
			,('admin.place.search', 'Chercher une place', @AdminTourn, 0)
			,('admin.place.reserve', 'Changer la r�servation d''une place', @AdminPlace, 0)
			,('admin.place.arriving', 'Confirmer la pr�sense d''une personne', @AdminPlace, 0)
			,('admin.place.leaving', 'Confirmer le d�part d''une personne', @AdminPlace, 0)
			,('admin.place.cancel', 'Annuler une r�servation', @AdminPlace, 0)

			,('admin.tournament.index', 'Afficher la liste des tournois', @AdminTourn, 0)
			,('admin.tournament.details', 'Afficher les d�tails d''un tournoi', @AdminTourn, 0)
			,('admin.tournament.create', 'Cr�er un tournoi', @AdminTourn, 0)
			,('admin.tournament.edit', 'Modifier un tournoi', @AdminTourn, 0)
			,('admin.tournament.delete', 'Supprimer un tournoi', @AdminTourn, 0)
			,('admin.tournament.team.details', 'Afficher les d�tails d''une �quipe', @AdminTourn, 0)
			,('admin.tournament.team.edit', 'Modifier une �quipe', @AdminTourn, 0)
			,('admin.tournament.team.delete', 'Supprimer une �quipe', @AdminTourn, 0)
			,('admin.tournament.team.kick', 'Enlever un joueur d''une �quipe', @AdminTourn, 0)

			,('admin.user.index', 'Liste des utilisateurs', @AdminTourn, 0)
			,('admin.user.details', 'D�tails d''un utilisateur', @AdminTourn, 0)
			,('admin.user.edit', 'Modifier un utilisateur', @AdminPlace, 0)
			,('admin.user.delete', 'Supprimer un utilisateur', @AdminPlace, 0)

/* ============================================== *
 *			Insertion des param�tres			  *
 *=============================================== */

 INSERT INTO [dbo].[Settings] (StartDate, EndDate, Rules, SendRememberEmail, NbDaysBeforeRemember, RememberEmailContent, Description)
	VALUES	('2015-10-14 12:00', '2015-10-15 12:00', '', 0, 0, '', '')

UPDATE [dbo].[Settings]
	SET Rules = '

	';

UPDATE [dbo].[Settings]
	SET Description = 'Le LAN de l''ADEPT est l''activit� organis�e par l''ADEPT Informatique, c''est � dire, les �tudiants et �tudiantes de la technique informatique du C�gep �douard-Montpetit. Cette activit� est organis�e � chaque session d''automne et d''hiver lors de la semaine de lecture.  

Bon LAN !';

UPDATE [dbo].[Settings]
	SET Rules = '';

UPDATE [dbo].[Settings]
	SET RememberEmailContent = '';


/* ============================================== *
 *			Initialisation des ID de users		  *
 *=============================================== */

 GO

 DBCC CHECKIDENT ('[dbo].[Users]', RESEED, 2964);

 GO

