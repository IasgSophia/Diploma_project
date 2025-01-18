USE [master]
GO
/****** Object:  Database [gallerydatabase]    Script Date: 19.01.2025 0:25:41 ******/
CREATE DATABASE [gallerydatabase]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'gallerydatabase', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\gallerydatabase.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'gallerydatabase_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\gallerydatabase_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [gallerydatabase] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [gallerydatabase].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [gallerydatabase] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [gallerydatabase] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [gallerydatabase] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [gallerydatabase] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [gallerydatabase] SET ARITHABORT OFF 
GO
ALTER DATABASE [gallerydatabase] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [gallerydatabase] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [gallerydatabase] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [gallerydatabase] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [gallerydatabase] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [gallerydatabase] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [gallerydatabase] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [gallerydatabase] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [gallerydatabase] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [gallerydatabase] SET  DISABLE_BROKER 
GO
ALTER DATABASE [gallerydatabase] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [gallerydatabase] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [gallerydatabase] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [gallerydatabase] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [gallerydatabase] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [gallerydatabase] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [gallerydatabase] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [gallerydatabase] SET RECOVERY FULL 
GO
ALTER DATABASE [gallerydatabase] SET  MULTI_USER 
GO
ALTER DATABASE [gallerydatabase] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [gallerydatabase] SET DB_CHAINING OFF 
GO
ALTER DATABASE [gallerydatabase] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [gallerydatabase] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [gallerydatabase] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [gallerydatabase] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'gallerydatabase', N'ON'
GO
ALTER DATABASE [gallerydatabase] SET QUERY_STORE = ON
GO
ALTER DATABASE [gallerydatabase] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [gallerydatabase]
GO
/****** Object:  Table [dbo].[Art]    Script Date: 19.01.2025 0:25:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Art](
	[id] [int] NOT NULL,
	[title] [nvarchar](100) NOT NULL,
	[author] [nvarchar](100) NOT NULL,
	[genre] [nvarchar](100) NOT NULL,
	[idTypeSize] [int] NOT NULL,
	[size] [nvarchar](100) NOT NULL,
	[price] [decimal](18, 0) NOT NULL,
	[idExibition] [int] NOT NULL,
	[Decription] [nvarchar](1000) NULL,
	[PhotoName] [nvarchar](50) NULL,
	[ProductPhoto] [image] NULL,
	[Comments] [nvarchar](1000) NULL,
	[IdWorker] [int] NULL,
 CONSTRAINT [PK_Art] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Clients]    Script Date: 19.01.2025 0:25:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Clients](
	[Id] [int] NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[MiddleName] [nvarchar](100) NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[Mail] [nvarchar](100) NOT NULL,
	[Phone] [nvarchar](100) NULL,
	[Login] [nvarchar](100) NOT NULL,
	[Password] [nvarchar](100) NOT NULL,
	[Birth] [date] NULL,
 CONSTRAINT [PK_Clients] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Exibition]    Script Date: 19.01.2025 0:25:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Exibition](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](400) NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NOT NULL,
 CONSTRAINT [PK_Exibition] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Order]    Script Date: 19.01.2025 0:25:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Order](
	[Id] [int] NOT NULL,
	[IdClient] [int] NOT NULL,
	[IdArt] [int] NOT NULL,
	[Comment] [nvarchar](1000) NULL,
	[Adress] [nvarchar](200) NOT NULL,
	[IdShippingType] [int] NOT NULL,
 CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Position]    Script Date: 19.01.2025 0:25:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Position](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Position] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 19.01.2025 0:25:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ShippingType]    Script Date: 19.01.2025 0:25:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShippingType](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_ShippingType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TypeSize]    Script Date: 19.01.2025 0:25:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TypeSize](
	[Id] [int] NOT NULL,
	[Size] [nvarchar](20) NULL,
 CONSTRAINT [PK_TypeSize] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Workers]    Script Date: 19.01.2025 0:25:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Workers](
	[Id] [int] NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[MiddleName] [nvarchar](100) NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[idPosition] [int] NOT NULL,
	[idRole] [int] NOT NULL,
	[Mail] [nvarchar](100) NOT NULL,
	[Phone] [nvarchar](100) NULL,
	[Login] [nvarchar](100) NOT NULL,
	[Password] [nvarchar](100) NOT NULL,
	[Birth] [date] NULL,
 CONSTRAINT [PK_Workers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (1, N'Река в сумерках', N'Альфред Сислей', N'Импрессионизм', 1, N'60x80 см', CAST(12000 AS Decimal(18, 0)), 1, N'Отражение заходящего солнца в воде.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (2, N'Парижские крыши', N'Гюстав Кайботт', N'Импрессионизм', 2, N'75x90 см', CAST(15000 AS Decimal(18, 0)), 1, N'Вид на крыши Парижа в теплых тонах.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (3, N'Солнечный сад', N'Камиль Писсарро', N'Импрессионизм', 3, N'80x100 см', CAST(18000 AS Decimal(18, 0)), 1, N'Пейзаж с солнечными бликами на листьях.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (4, N'Туманное утро', N'Берт Моризо', N'Импрессионизм', 4, N'60x70 см', CAST(10000 AS Decimal(18, 0)), 1, N'Рассветная дымка в сельской местности.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (5, N'Лодки на якоре', N'Эжен Буден', N'Импрессионизм', 1, N'50x70 см', CAST(11000 AS Decimal(18, 0)), 1, N'Яхты на стоянке у побережья.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (6, N'Нежный ветерок', N'Мэри Кассат', N'Импрессионизм', 2, N'65x85 см', CAST(14000 AS Decimal(18, 0)), 1, N'Сцена пикника в парке.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (7, N'Улица в Монмартре', N'Поль Синьяк', N'Постимпрессионизм', 3, N'70x90 см', CAST(16000 AS Decimal(18, 0)), 1, N'Городская улица в Монмартре.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (8, N'Полуденные тени', N'Жан Ренуар', N'Импрессионизм', 4, N'55x75 см', CAST(12000 AS Decimal(18, 0)), 1, N'Тени деревьев в жаркий день.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (9, N'Поле лаванды', N'Анри-Эдмон Кросс', N'Постимпрессионизм', 1, N'80x110 см', CAST(20000 AS Decimal(18, 0)), 1, N'Цветущее лавандовое поле.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (10, N'Отражения в Сене', N'Арман Гийомен', N'Импрессионизм', 2, N'90x120 см', CAST(22000 AS Decimal(18, 0)), 1, N'Отражения в реке Сена.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (11, N'Городские силуэты', N'Жан-Мишель Баския', N'Неоэкспрессионизм', 3, N'80x100 см', CAST(35000 AS Decimal(18, 0)), 2, N'Силуэты зданий, вписанные в хаотичный фон.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (12, N'Цвет в хаосе', N'Герхард Рихтер', N'Абстракционизм', 4, N'100x120 см', CAST(45000 AS Decimal(18, 0)), 2, N'Сложная абстрактная работа.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (13, N'Неоновый горизонт', N'Такаши Мураками', N'Поп Арт', 1, N'75x95 см', CAST(38000 AS Decimal(18, 0)), 2, N'Неоновое свечение в городском пейзаже.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (14, N'Фрагментированный город', N'Дэвид Хокни', N'Поп Арт', 2, N'85x110 см', CAST(40000 AS Decimal(18, 0)), 2, N'Город в разноцветных плоскостях.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (15, N'Цифровая симфония', N'Дженни Хольцер', N'Концептуализм', 3, N'120x150 см', CAST(50000 AS Decimal(18, 0)), 2, N'Инсталляция с цифровыми текстами.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (16, N'Эфемерные формы', N'Дэмьен Херст', N'Абстракционизм', 4, N'90x130 см', CAST(47000 AS Decimal(18, 0)), 2, N'Орнаменты из геометрических фигур.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (17, N'Через экран', N'Яёи Кусама', N'Концептуализм', 1, N'70x100 см', CAST(42000 AS Decimal(18, 0)), 2, N'Тема отражения и искажения.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (18, N'Пульс города', N'Бэнкси', N'Неоэкспрессионизм', 2, N'50x70 см', CAST(30000 AS Decimal(18, 0)), 2, N'Динамика городских стен.', N'city_pulse.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (19, N'Синтетические ландшафты', N'Тобиас Рехбергер', N'Абстракционизм', 3, N'60x90 см', CAST(34000 AS Decimal(18, 0)), 2, N'Нереальные пейзажи.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (20, N'Пересечение', N'Барбара Крюгер', N'Концептуализм', 4, N'100x140 см', CAST(49000 AS Decimal(18, 0)), 2, N'Социальная критика.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (21, N'Поля золота', N'Анри Матисс', N'Постимпрессионизм', 1, N'70x90 см', CAST(30000 AS Decimal(18, 0)), 3, N'Пейзаж с желтыми тонами.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (22, N'Джазовые волны', N'Пабло Пикассо', N'Кубизм', 2, N'80x100 см', CAST(40000 AS Decimal(18, 0)), 3, N'Работа с джазовой тематикой.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (23, N'Современные отголоски', N'Марк Ротко', N'Абстракционизм', 3, N'90x110 см', CAST(45000 AS Decimal(18, 0)), 3, N'Мягкие цветовые переходы.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (24, N'Текущие цвета', N'Пауль Клее', N'Абстракционизм', 4, N'60x80 см', CAST(35000 AS Decimal(18, 0)), 3, N'Яркий абстрактный сюжет.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (25, N'Шёпот океана', N'Хелен Франкенталер', N'Абстракционизм', 1, N'100x120 см', CAST(47000 AS Decimal(18, 0)), 3, N'Морская тематика.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (26, N'Симфония форм', N'Василий Кандинский', N'Абстракционизм', 2, N'85x115 см', CAST(39000 AS Decimal(18, 0)), 3, N'Сложные геометрические структуры.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (27, N'Вечное сияние', N'Жоан Миро', N'Сюрреализм', 3, N'70x100 см', CAST(37000 AS Decimal(18, 0)), 3, N'Слияние символов и цветов.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (28, N'Абстрактная мелодия', N'Джексон Поллок', N'Абстракционизм', 4, N'90x120 см', CAST(45000 AS Decimal(18, 0)), 3, N'Динамичные линии.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (29, N'Свет через стекло', N'Робер Делоне', N'Кубизм', 1, N'75x95 см', CAST(32000 AS Decimal(18, 0)), 3, N'Сложные формы и цвета.', N'1.jpg', NULL, NULL, NULL)
INSERT [dbo].[Art] ([id], [title], [author], [genre], [idTypeSize], [size], [price], [idExibition], [Decription], [PhotoName], [ProductPhoto], [Comments], [IdWorker]) VALUES (30, N'Гармония противоположностей', N'Жорж Брак', N'Кубизм', 2, N'80x110 см', CAST(41000 AS Decimal(18, 0)), 3, N'Контрасты и баланс.', N'1.jpg', NULL, NULL, NULL)
GO
INSERT [dbo].[Clients] ([Id], [FirstName], [MiddleName], [LastName], [Mail], [Phone], [Login], [Password], [Birth]) VALUES (1, N'Александр', N'Владимирович', N'Петров', N'alex.petrov@mail.com', N'+79870000001', N'alex.petrov', N'password1', CAST(N'1980-02-15' AS Date))
INSERT [dbo].[Clients] ([Id], [FirstName], [MiddleName], [LastName], [Mail], [Phone], [Login], [Password], [Birth]) VALUES (2, N'Мария', N'Сергеевна', N'Иванова', N'maria.ivanova@mail.com', N'+79870000002', N'maria.ivanova', N'password2', CAST(N'1992-07-10' AS Date))
INSERT [dbo].[Clients] ([Id], [FirstName], [MiddleName], [LastName], [Mail], [Phone], [Login], [Password], [Birth]) VALUES (3, N'Олег', N'Николаевич', N'Смирнов', N'oleg.smirnov@mail.com', N'+79870000003', N'oleg.smirnov', N'password3', CAST(N'1987-12-03' AS Date))
INSERT [dbo].[Clients] ([Id], [FirstName], [MiddleName], [LastName], [Mail], [Phone], [Login], [Password], [Birth]) VALUES (4, N'Анна', N'Викторовна', N'Соколова', N'anna.sokolova@mail.com', N'+79870000004', N'anna.sokolova', N'password4', CAST(N'1990-05-21' AS Date))
INSERT [dbo].[Clients] ([Id], [FirstName], [MiddleName], [LastName], [Mail], [Phone], [Login], [Password], [Birth]) VALUES (5, N'Дмитрий', N'Александрович', N'Морозов', N'dmitry.morozov@mail.com', N'+79870000005', N'dmitry.morozov', N'password5', CAST(N'1985-08-30' AS Date))
INSERT [dbo].[Clients] ([Id], [FirstName], [MiddleName], [LastName], [Mail], [Phone], [Login], [Password], [Birth]) VALUES (6, N'Елена', N'Павловна', N'Федорова', N'elena.fedorova@mail.com', N'+79870000006', N'elena.fedorova', N'password6', CAST(N'1995-01-25' AS Date))
INSERT [dbo].[Clients] ([Id], [FirstName], [MiddleName], [LastName], [Mail], [Phone], [Login], [Password], [Birth]) VALUES (7, N'Иван', N'Степанович', N'Кузнецов', N'ivan.kuznetsov@mail.com', N'+79870000007', N'ivan.kuznetsov', N'password7', CAST(N'1982-03-14' AS Date))
INSERT [dbo].[Clients] ([Id], [FirstName], [MiddleName], [LastName], [Mail], [Phone], [Login], [Password], [Birth]) VALUES (8, N'Татьяна', N'Михайловна', N'Васильева', N'tatiana.vasilieva@mail.com', N'+79870000008', N'tatiana.vasilieva', N'password8', CAST(N'1993-10-09' AS Date))
INSERT [dbo].[Clients] ([Id], [FirstName], [MiddleName], [LastName], [Mail], [Phone], [Login], [Password], [Birth]) VALUES (9, N'Максим', N'Юрьевич', N'Зайцев', N'maxim.zaitsev@mail.com', N'+79870000009', N'maxim.zaitsev', N'password9', CAST(N'1991-06-18' AS Date))
INSERT [dbo].[Clients] ([Id], [FirstName], [MiddleName], [LastName], [Mail], [Phone], [Login], [Password], [Birth]) VALUES (10, N'Оксана', N'Игоревна', N'Новикова', N'oksana.novikova@mail.com', N'+79870000010', N'oksana.novikova', N'password10', CAST(N'1988-11-27' AS Date))
GO
INSERT [dbo].[Exibition] ([Id], [Name], [Description], [StartDate], [EndDate]) VALUES (1, N'Echoes of Light', N'Выставка произведений импрессионистов, акцент на игре света и оттенков.', CAST(N'2025-02-01' AS Date), CAST(N'2025-03-15' AS Date))
INSERT [dbo].[Exibition] ([Id], [Name], [Description], [StartDate], [EndDate]) VALUES (2, N'Contemporary Whispers', N'Современные работы, исследующие динамику городов и социальные темы.', CAST(N'2025-04-01' AS Date), CAST(N'2025-05-15' AS Date))
INSERT [dbo].[Exibition] ([Id], [Name], [Description], [StartDate], [EndDate]) VALUES (3, N'The Fusion', N'Коллекция, объединяющая современное искусство и влияние импрессионизма.', CAST(N'2025-06-01' AS Date), CAST(N'2025-07-15' AS Date))
GO
INSERT [dbo].[Order] ([Id], [IdClient], [IdArt], [Comment], [Adress], [IdShippingType]) VALUES (1, 1, 1, N'Пожалуйста, доставьте картину в будний день, чтобы я был дома. Важно не повредить рамку при транспортировке.', N'Москва, ул. Ленина, 15', 2)
INSERT [dbo].[Order] ([Id], [IdClient], [IdArt], [Comment], [Adress], [IdShippingType]) VALUES (2, 2, 2, NULL, N'Санкт-Петербург, ул. Пушкина, 10', 1)
INSERT [dbo].[Order] ([Id], [IdClient], [IdArt], [Comment], [Adress], [IdShippingType]) VALUES (3, 3, 3, N'Предпочитаю, чтобы доставка была в первую половину дня. Пожалуйста, уточните время.', N'Екатеринбург, ул. Вокзальная, 5', 2)
INSERT [dbo].[Order] ([Id], [IdClient], [IdArt], [Comment], [Adress], [IdShippingType]) VALUES (4, 4, 4, NULL, N'Новосибирск, ул. Пионерская, 7', 3)
INSERT [dbo].[Order] ([Id], [IdClient], [IdArt], [Comment], [Adress], [IdShippingType]) VALUES (5, 5, 5, NULL, N'Ростов-на-Дону, ул. Красная, 12', 1)
INSERT [dbo].[Order] ([Id], [IdClient], [IdArt], [Comment], [Adress], [IdShippingType]) VALUES (6, 6, 6, NULL, N'Воронеж, ул. Советская, 20', 2)
INSERT [dbo].[Order] ([Id], [IdClient], [IdArt], [Comment], [Adress], [IdShippingType]) VALUES (7, 7, 7, NULL, N'Уфа, ул. Горького, 18', 3)
INSERT [dbo].[Order] ([Id], [IdClient], [IdArt], [Comment], [Adress], [IdShippingType]) VALUES (8, 8, 8, NULL, N'Казань, ул. Мира, 9', 1)
INSERT [dbo].[Order] ([Id], [IdClient], [IdArt], [Comment], [Adress], [IdShippingType]) VALUES (9, 9, 9, NULL, N'Самара, ул. Лесная, 22', 2)
INSERT [dbo].[Order] ([Id], [IdClient], [IdArt], [Comment], [Adress], [IdShippingType]) VALUES (10, 10, 10, N'Буду рад, если доставка будет выполнена в выходные. Возможно, нужна помощь при подъеме на этаж.', N'Краснодар, ул. Пушкина, 3', 1)
GO
INSERT [dbo].[Position] ([Id], [Name]) VALUES (1, N'Менеджер')
INSERT [dbo].[Position] ([Id], [Name]) VALUES (2, N'Куратор')
INSERT [dbo].[Position] ([Id], [Name]) VALUES (3, N'Директор')
INSERT [dbo].[Position] ([Id], [Name]) VALUES (4, N'Администратор')
INSERT [dbo].[Position] ([Id], [Name]) VALUES (5, N'Архитектор')
GO
INSERT [dbo].[Role] ([Id], [Name]) VALUES (1, N'Сотрудник')
INSERT [dbo].[Role] ([Id], [Name]) VALUES (2, N'Администратор')
GO
INSERT [dbo].[ShippingType] ([Id], [Name]) VALUES (1, N'Экспресс')
INSERT [dbo].[ShippingType] ([Id], [Name]) VALUES (2, N'Стандарт')
INSERT [dbo].[ShippingType] ([Id], [Name]) VALUES (3, N'Самовывоз')
INSERT [dbo].[ShippingType] ([Id], [Name]) VALUES (4, N'Международная')
GO
INSERT [dbo].[TypeSize] ([Id], [Size]) VALUES (1, N'Маленький')
INSERT [dbo].[TypeSize] ([Id], [Size]) VALUES (2, N'Средний')
INSERT [dbo].[TypeSize] ([Id], [Size]) VALUES (3, N'Большой')
INSERT [dbo].[TypeSize] ([Id], [Size]) VALUES (4, N'Огромный')
GO
INSERT [dbo].[Workers] ([Id], [FirstName], [MiddleName], [LastName], [idPosition], [idRole], [Mail], [Phone], [Login], [Password], [Birth]) VALUES (1, N'Аркадий', N'Геннадьевич', N'Шмидт-Беккер', 1, 1, N'arkady.shmidt@mail.com', N'+79870000001', N'arkady.shmidt', N'password123', CAST(N'1980-03-15' AS Date))
INSERT [dbo].[Workers] ([Id], [FirstName], [MiddleName], [LastName], [idPosition], [idRole], [Mail], [Phone], [Login], [Password], [Birth]) VALUES (2, N'Зинаида', N'Петровна', N'Вжухова', 2, 1, N'zinaida.vzhukhova@mail.com', N'+79870000002', N'zinaida.vzhukhova', N'password456', CAST(N'1991-07-20' AS Date))
INSERT [dbo].[Workers] ([Id], [FirstName], [MiddleName], [LastName], [idPosition], [idRole], [Mail], [Phone], [Login], [Password], [Birth]) VALUES (3, N'Юлиан', N'Васильевич', N'Зубоскал', 3, 1, N'yulian.zuboskal@mail.com', N'+79870000003', N'yulian.zuboskal', N'password789', CAST(N'1985-11-11' AS Date))
INSERT [dbo].[Workers] ([Id], [FirstName], [MiddleName], [LastName], [idPosition], [idRole], [Mail], [Phone], [Login], [Password], [Birth]) VALUES (4, N'Мирослав', N'Юрьевич', N'Чеховский', 4, 1, N'miroslav.chekhovskiy@mail.com', N'+79870000004', N'miroslav.chekhovskiy', N'password101', CAST(N'1990-05-05' AS Date))
INSERT [dbo].[Workers] ([Id], [FirstName], [MiddleName], [LastName], [idPosition], [idRole], [Mail], [Phone], [Login], [Password], [Birth]) VALUES (5, N'Тимофей', N'Андреевич', N'Строганов-Сергеев', 1, 1, N'timofey.stroganov@mail.com', N'+79870000005', N'timofey.stroganov', N'password112', CAST(N'1988-02-28' AS Date))
INSERT [dbo].[Workers] ([Id], [FirstName], [MiddleName], [LastName], [idPosition], [idRole], [Mail], [Phone], [Login], [Password], [Birth]) VALUES (6, N'Дарина', N'Максимовна', N'Бубновская', 2, 1, N'darina.bubnovskaya@mail.com', N'+79870000006', N'darina.bubnovskaya', N'password134', CAST(N'1993-08-22' AS Date))
INSERT [dbo].[Workers] ([Id], [FirstName], [MiddleName], [LastName], [idPosition], [idRole], [Mail], [Phone], [Login], [Password], [Birth]) VALUES (7, N'Виктор', N'Дмитриевич', N'Перцевский', 3, 1, N'victor.percev@mail.com', N'+79870000007', N'victor.percev', N'password145', CAST(N'1983-06-14' AS Date))
INSERT [dbo].[Workers] ([Id], [FirstName], [MiddleName], [LastName], [idPosition], [idRole], [Mail], [Phone], [Login], [Password], [Birth]) VALUES (8, N'Леонард', N'Евгеньевич', N'Щербатов', 2, 1, N'leonard.scherbatov@mail.com', N'+79870000012', N'leonard.scherbatov', N'password233', CAST(N'1990-09-10' AS Date))
INSERT [dbo].[Workers] ([Id], [FirstName], [MiddleName], [LastName], [idPosition], [idRole], [Mail], [Phone], [Login], [Password], [Birth]) VALUES (9, N'Нина', N'Михайловна', N'Барбарова', 1, 1, N'nina.barbarova@mail.com', N'+79870000015', N'nina.barbarova', N'password289', CAST(N'1995-10-22' AS Date))
INSERT [dbo].[Workers] ([Id], [FirstName], [MiddleName], [LastName], [idPosition], [idRole], [Mail], [Phone], [Login], [Password], [Birth]) VALUES (10, N'Егор', N'Юрьевич', N'Чернышев', 2, 2, N'egor.chernyshev@mail.com', N'+79870000016', N'egor.chernyshev', N'password301', CAST(N'1989-03-12' AS Date))
INSERT [dbo].[Workers] ([Id], [FirstName], [MiddleName], [LastName], [idPosition], [idRole], [Mail], [Phone], [Login], [Password], [Birth]) VALUES (11, N'Марина', N'Анатольевна', N'Пашкевич', 3, 2, N'marina.pashkevich@mail.com', N'+79870000017', N'marina.pashkevich', N'password313', CAST(N'1994-05-04' AS Date))
INSERT [dbo].[Workers] ([Id], [FirstName], [MiddleName], [LastName], [idPosition], [idRole], [Mail], [Phone], [Login], [Password], [Birth]) VALUES (12, N'Оксана', N'Валентиновна', N'Гудкова', 4, 2, N'oksana.gudkova@mail.com', N'+79870000018', N'oksana.gudkova', N'password325', CAST(N'1991-08-18' AS Date))
GO
ALTER TABLE [dbo].[Art]  WITH CHECK ADD  CONSTRAINT [FK_Art_Exibition] FOREIGN KEY([idExibition])
REFERENCES [dbo].[Exibition] ([Id])
GO
ALTER TABLE [dbo].[Art] CHECK CONSTRAINT [FK_Art_Exibition]
GO
ALTER TABLE [dbo].[Art]  WITH CHECK ADD  CONSTRAINT [FK_Art_TypeSize] FOREIGN KEY([idTypeSize])
REFERENCES [dbo].[TypeSize] ([Id])
GO
ALTER TABLE [dbo].[Art] CHECK CONSTRAINT [FK_Art_TypeSize]
GO
ALTER TABLE [dbo].[Art]  WITH CHECK ADD  CONSTRAINT [FK_Art_Workers] FOREIGN KEY([IdWorker])
REFERENCES [dbo].[Workers] ([Id])
GO
ALTER TABLE [dbo].[Art] CHECK CONSTRAINT [FK_Art_Workers]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_Art] FOREIGN KEY([IdArt])
REFERENCES [dbo].[Art] ([id])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_Art]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_Clients] FOREIGN KEY([IdClient])
REFERENCES [dbo].[Clients] ([Id])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_Clients]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_ShippingType] FOREIGN KEY([IdShippingType])
REFERENCES [dbo].[ShippingType] ([Id])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_ShippingType]
GO
ALTER TABLE [dbo].[Workers]  WITH CHECK ADD  CONSTRAINT [FK_Workers_Position] FOREIGN KEY([idPosition])
REFERENCES [dbo].[Position] ([Id])
GO
ALTER TABLE [dbo].[Workers] CHECK CONSTRAINT [FK_Workers_Position]
GO
ALTER TABLE [dbo].[Workers]  WITH CHECK ADD  CONSTRAINT [FK_Workers_Role] FOREIGN KEY([idRole])
REFERENCES [dbo].[Role] ([Id])
GO
ALTER TABLE [dbo].[Workers] CHECK CONSTRAINT [FK_Workers_Role]
GO
USE [master]
GO
ALTER DATABASE [gallerydatabase] SET  READ_WRITE 
GO
