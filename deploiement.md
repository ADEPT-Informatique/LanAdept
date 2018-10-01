# Guide de déploiement du site web sur Azure
La méthode la plus simple pour héberger le site web est d'utiliser Microsoft Azure (Azure SQL Server et Azure Web Apps). Ce guide vous expliquera comment préparer le serveur et la base de donnée, effectuer sa migration vers Azure et déployer les deux projets (LanAdept et LanAdeptAdmin).


## Prérequis
Créer un compte microsoft et un abonnement Azure

> Il existe différents moyens de créer un abonnement Azure gratuitement pour la durée du LAN. Au moment où ce guide est écrit, en octobre 2018, il est possible d'obtenir un abonnement Azure for Students (et un crédit de 100$) en étant étudiant : https://azure.microsoft.com/en-us/free/students/. Sinon, un abonnement peut être créé dans `Accueil > Abonnements`

## 1. Préparation des ressources Azure

Connectez vous au portail azure avec le compte préalablement créé: https://portal.azure.com

### 1.1 Créer la BD et son serveur

- Dans la section *Créer une ressource*, sélectionner **SQL Database**
- Entrer le nom de la base de donnée, ex: `LanAdept`
- Selectionner l'abonnement *créé au prélable*, ex: `Azure for Student`
- Créer un nouveau **groupe de ressource**, ex: `LAN-A18`
- Sélectionner `Base de données vide` comme source
- Sélectionner `Créer un serveur` pour cette BD
    - entrer le nom du serveur, ex: `lanadept`
    - créer un compte pour le serveur, ex: `adept`
    - créer un mot de passe pour le serveur
    - choisir l'emplacement `East Canada`
    - cocher la case `Autoriser les services Azure à accéder au serveur`
- S'assurer que le serveur de la BD est bien celui que nous venons de créer
- Ne pas utiliser le pool élastique SQL
- Choisir le *Niveau tarifaire* de base
- Laisaer le Classement (encodage) par défaut
- Appuyer sur Créer

> Le serveur et la base de donnée seront déployés automatiquement en quelques minutes. Vous recevrez une notification lorsque ce sera pret.

 Le serveur et la BD apparaitron dans la section `Toutes les ressources` du tableau de bord.

 Sélectionnez la Base de donnée **LanAdept** (pas le serveur) et vérifiez que le statut est bien à `En ligne`. Notez aussi l'adresse du serveur. Dans notre exemple : `lanadept.database.windows.net`

 :bulb: **Vous devriez à ce point pouvoir vous connecter au serveur avec SQL Server Management Studio, en utilisant l'adresse `lanadept.database.windows.net`**

### 1.2 Créer une Web App pour le site principal

- Dans la section *Créer une ressource*, sélectionner **Web App**
- Entrer le nom de l'application : `lanadept`
- Sélectionner le même abonnement que pour la BD
- Sélectionner le groupe de ressource créé à l'étape précédente (`LAN-A18`)
- Choisir `Windows` comme OS
- Choisir l'option `Code` comme option de publication
- Créer un **Plan App Service** s'il n'y en a pas déjà un existant
    - Choisir un nom pour le plan
    - Choisir l'emplacement `East Canada`
    - Choisir le niveau tarifaire `F1 (Gratuit)` ou `D1`
- Désactiver Application Insights
- Cliquer sur Créer

> La web app sera seront déployés automatiquement en quelques secondes. Vous recevrez une notification lorsque ce sera pret.

### 1.3 Créer une Web App pour le site d'administration

Le processus est le même qu'a l'étape précédente, mais avec pour seule différence le nom de l'application (`adminlanadept`). Choisir le même *Groupe de ressource*, le même *Abonnement* et le même  *Plan App Service*


### 1.4 Ajuster le fuseau horaire de la Web App du site principal

Le fuseau horaire de la web app n'est pas nécéssairement la bonne il faut donc spécifier le bon fuseau horaire pour éviter les problèmes liés aux dates.

- Depuis le Tableau de bord, sélectionner la web app (App Service) `lanadept`
- Dans la section *Paramètres de l'application* cliquer sur *Ajouter un nouveau paramètre*
    - Nom du paramêtre : `WEBSITE_TIME_ZONE`
    - Valeur : `Eastern Standard Time`

## 2. Préparer l'application LanAdept au déploiement

:bulb: Ces instructions sont données pour Visual Studio 2017.

### 2.1 Synchroniser les packages et modifier la connexion string

- Cloner la solution depuis GitHub et l'ouvrir dans VS2017
- Regénérer la solution et synchroniser les pakages NuGets (télécharger les dependances)
- Ouvrir le fichier `LanAdept/Web.config`

Modifier la connexion string :

```xml
<connectionStrings>
    <add name="LanAdeptDataContext" connectionString="Server=tcp:{ADRESSE_DU_SERVEUR},1433;Initial Catalog={NOM_BD};Persist Security Info=False;User ID={UTILISATEUR};Password={MOT_DE_PASSE};MultipleActiveResultSets=False;Encrypt=True;" providerName="System.Data.SqlClient" />		
</connectionStrings>
```
Il faut entrer les informations de connexion au serveur créé à la section 1.1. Par exemple:

```xml
<connectionStrings>
    <add name="LanAdeptDataContext" connectionString="Server=tcp:lanadept.database.windows.net,1433;Initial Catalog=LanAdept;Persist Security Info=False;User ID=adept;Password=B0nj0urAT0us;MultipleActiveResultSets=False;Encrypt=True;" providerName="System.Data.SqlClient" />		
</connectionStrings>
```

Sauvegarder le fichier `Web.config`
- Dans la console du gestionaire NuGet **sélectionner le projet par défaut : LanAdeptData** et lancer la migration `> update-database`

Une fois la migration terminée, lancer l'application depuis Visual Studio. 
Si l'application se lance correctement, tous est prêt pour le déploiement du projet LanAdept. 
Sinon, vérifiez votre connexion string.

### 2.2 Déployer l'application sur Azure

- Dans l'explorateur de solution, clic-droit sur le projet `LanAdept` > `Publier`.
- Choisir `App service` (première option à gauche)
- Cocher `Sélectionner existant`
- Se connecter avec son compte Azure
- Sélectionné l'app service que nous avons créé précédement : `lanadept`

L'application s'ouvre automatiquement sur la page web hébergée (`https://lanadept.azurewebsites.net`)

## 3.1 Préparer l'application LanAdeptAdmin au déploiement

Tout ce qu'il reste à faire c'est d'ajouter la connexion string modifié précédement dans le projet `LanAdept` au fichier
`LanAdeptAdmin/Web.config`.

Je vous conseille de tester la connexion de l'app d'administration au serveur de BD aussi avant le déploiement

:bulb: Pour changer le projet de démarage, faire un clic droit sur le projet `LanAdeptAdmin` et cliquer sur `Définir comme projet de démarage`

Si l'application se lance correctement, tous est prêt pour le déploiement du projet LanAdeptAdmin. 
Sinon, vérifiez votre connexion string.

### 3.2 Déployer l'application sur Azure

- Dans l'explorateur de solution, clic-droit sur le projet `LanAdeptAdmin` > `Publier`.
- Choisir `App service` (première option à gauche)
- Cocher `Sélectionner existant`
- Sélectionné le second app service que nous avons créé précédement : `adminlanadept`

L'application s'ouvre automatiquement sur la page web hébergée (`https://adminlanadept.azurewebsites.net`)

### 4. Post-déploiement

Les deux projets sont déployés et en ligne ! 
Cependant il nous faut ajouter manuellement un premier administrateur `Owner` à la BD.

#### Création d'un compte administrateur `Owner`

Depuis SSMS, se connecter au serveur `lanadept.database.windows.net`

Il faudra éxécuter deux requêtes.

D'abord insérez un utilisateur :
```sql
INSERT [dbo].[AspNetUsers] ([ID],[CompleteName], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'c8aaf2f7-ee34-4075-8da7-eaf0deda966d',N'VOTRE_NOM', N'VOTRE_EMAIL', 1, N'AJSwwY4q+gb66ZxmOhPJwrvfFtSdJ9ds7XUYNc0+kdg8ruYjFj1H9h9lkhYwg8LXag==', N'45151331-b44d-482f-85d5-11d3c9a6feb5', NULL, 0, 0, NULL, 1, 0, N'VOTRE_EMAIL')
```
:bulb: ***Le mot de passe de ce compte sera `admin123`** et pourra être changé éventuellement depuis le site. 

Donnez le rôle `owner` à cet utilisateur :

```sql
DECLARE @idOwner nvarchar(40);
SELECT @idOwner = id FROM AspNetRoles WHERE Name = 'owner';
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c8aaf2f7-ee34-4075-8da7-eaf0deda966d', @idOwner);
```

Et voilà ! Le déploiement est terminé !
Il ne reste plus qu'à modifier les paramètres du prochain LAN (description, dates, règles, seats.io) depuis le site d'administration !

Si vous avez d'autres question n'ésitez pas à me contacter! 
[@obrassard](https://github.com/obrassard)





