USE [AditOAUTH]
GO
/****** Object:  Table [dbo].[oauth_client_endpoints]    Script Date: 27/01/2014 14:38:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[oauth_client_endpoints](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[client_id] [nvarchar](40) NOT NULL,
	[uri_protocol] [nvarchar](5) NOT NULL,
	[uri_domain] [nvarchar](128) NOT NULL,
	[uri_port] [int] NULL,
	[uri_path] [nvarchar](max) NULL,
 CONSTRAINT [PK_oauth_client_endpoints] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[oauth_clients]    Script Date: 27/01/2014 14:38:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[oauth_clients](
	[id] [nvarchar](40) NOT NULL,
	[secret] [nvarchar](40) NOT NULL,
	[name] [nvarchar](256) NOT NULL,
	[auto_approve] [bit] NOT NULL,
 CONSTRAINT [PK_oauth_clients] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[oauth_scopes]    Script Date: 27/01/2014 14:38:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[oauth_scopes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[scope] [nvarchar](256) NOT NULL,
	[name] [nvarchar](256) NOT NULL,
	[description] [nvarchar](max) NULL,
 CONSTRAINT [PK_oauth_scopes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[oauth_session_access_tokens]    Script Date: 27/01/2014 14:38:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[oauth_session_access_tokens](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[session_id] [int] NOT NULL,
	[access_token] [nvarchar](128) NOT NULL,
	[access_token_expires] [datetime] NOT NULL,
 CONSTRAINT [PK_oauth_session_access_tokens] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[oauth_session_authcode_scopes]    Script Date: 27/01/2014 14:38:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[oauth_session_authcode_scopes](
	[oauth_session_authcode_id] [int] NOT NULL,
	[scope_id] [int] NOT NULL,
 CONSTRAINT [PK_oauth_session_authcode_scopes] PRIMARY KEY CLUSTERED 
(
	[oauth_session_authcode_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[oauth_session_authcodes]    Script Date: 27/01/2014 14:38:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[oauth_session_authcodes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[session_id] [int] NOT NULL,
	[auth_code] [nvarchar](128) NOT NULL,
	[auth_code_expires] [datetime] NOT NULL,
 CONSTRAINT [PK_oauth_session_authcodes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[oauth_session_redirects]    Script Date: 27/01/2014 14:38:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[oauth_session_redirects](
	[session_id] [int] NOT NULL,
	[redirect_uri] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_oauth_session_redirects] PRIMARY KEY CLUSTERED 
(
	[session_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[oauth_session_refresh_tokens]    Script Date: 27/01/2014 14:38:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[oauth_session_refresh_tokens](
	[session_access_token_id] [int] NOT NULL,
	[refresh_token] [nvarchar](40) NOT NULL,
	[refresh_token_expires] [datetime] NOT NULL,
	[client_id] [nvarchar](40) NOT NULL,
 CONSTRAINT [PK_oauth_session_refresh_tokens] PRIMARY KEY CLUSTERED 
(
	[session_access_token_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[oauth_session_token_scopes]    Script Date: 27/01/2014 14:38:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[oauth_session_token_scopes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[session_access_token_id] [int] NULL,
	[scope_id] [int] NOT NULL,
 CONSTRAINT [PK_oauth_session_token_scopes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[oauth_sessions]    Script Date: 27/01/2014 14:38:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[oauth_sessions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[client_id] [nvarchar](40) NOT NULL,
	[owner_type] [nvarchar](10) NOT NULL,
	[owner_id] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_oauth_sessions] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[oauth_clients] ADD  CONSTRAINT [DF_oauth_clients_auto_approve]  DEFAULT ((0)) FOR [auto_approve]
GO
ALTER TABLE [dbo].[oauth_sessions] ADD  CONSTRAINT [DF_oauth_sessions_owner_type]  DEFAULT ('user') FOR [owner_type]
GO
ALTER TABLE [dbo].[oauth_client_endpoints]  WITH CHECK ADD  CONSTRAINT [FK_oauth_client_endpoints_oauth_clients] FOREIGN KEY([client_id])
REFERENCES [dbo].[oauth_clients] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oauth_client_endpoints] CHECK CONSTRAINT [FK_oauth_client_endpoints_oauth_clients]
GO
ALTER TABLE [dbo].[oauth_session_access_tokens]  WITH CHECK ADD  CONSTRAINT [FK_oauth_session_access_tokens_oauth_sessions] FOREIGN KEY([session_id])
REFERENCES [dbo].[oauth_sessions] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oauth_session_access_tokens] CHECK CONSTRAINT [FK_oauth_session_access_tokens_oauth_sessions]
GO
ALTER TABLE [dbo].[oauth_session_authcode_scopes]  WITH CHECK ADD  CONSTRAINT [FK_oauth_session_authcode_scopes_oauth_scopes] FOREIGN KEY([scope_id])
REFERENCES [dbo].[oauth_scopes] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oauth_session_authcode_scopes] CHECK CONSTRAINT [FK_oauth_session_authcode_scopes_oauth_scopes]
GO
ALTER TABLE [dbo].[oauth_session_authcode_scopes]  WITH CHECK ADD  CONSTRAINT [FK_oauth_session_authcode_scopes_oauth_session_authcodes] FOREIGN KEY([oauth_session_authcode_id])
REFERENCES [dbo].[oauth_session_authcodes] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oauth_session_authcode_scopes] CHECK CONSTRAINT [FK_oauth_session_authcode_scopes_oauth_session_authcodes]
GO
ALTER TABLE [dbo].[oauth_session_authcodes]  WITH CHECK ADD  CONSTRAINT [FK_oauth_session_authcodes_oauth_sessions] FOREIGN KEY([session_id])
REFERENCES [dbo].[oauth_sessions] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oauth_session_authcodes] CHECK CONSTRAINT [FK_oauth_session_authcodes_oauth_sessions]
GO
ALTER TABLE [dbo].[oauth_session_redirects]  WITH CHECK ADD  CONSTRAINT [FK_oauth_session_redirects_oauth_sessions] FOREIGN KEY([session_id])
REFERENCES [dbo].[oauth_sessions] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oauth_session_redirects] CHECK CONSTRAINT [FK_oauth_session_redirects_oauth_sessions]
GO
ALTER TABLE [dbo].[oauth_session_refresh_tokens]  WITH CHECK ADD  CONSTRAINT [FK_oauth_session_refresh_tokens_oauth_clients] FOREIGN KEY([client_id])
REFERENCES [dbo].[oauth_clients] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oauth_session_refresh_tokens] CHECK CONSTRAINT [FK_oauth_session_refresh_tokens_oauth_clients]
GO
ALTER TABLE [dbo].[oauth_session_refresh_tokens]  WITH CHECK ADD  CONSTRAINT [FK_oauth_session_refresh_tokens_oauth_session_access_tokens] FOREIGN KEY([session_access_token_id])
REFERENCES [dbo].[oauth_session_access_tokens] ([id])
GO
ALTER TABLE [dbo].[oauth_session_refresh_tokens] CHECK CONSTRAINT [FK_oauth_session_refresh_tokens_oauth_session_access_tokens]
GO
ALTER TABLE [dbo].[oauth_session_token_scopes]  WITH CHECK ADD  CONSTRAINT [FK_oauth_session_token_scopes_oauth_scopes] FOREIGN KEY([scope_id])
REFERENCES [dbo].[oauth_scopes] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oauth_session_token_scopes] CHECK CONSTRAINT [FK_oauth_session_token_scopes_oauth_scopes]
GO
ALTER TABLE [dbo].[oauth_session_token_scopes]  WITH CHECK ADD  CONSTRAINT [FK_oauth_session_token_scopes_oauth_session_access_tokens] FOREIGN KEY([session_access_token_id])
REFERENCES [dbo].[oauth_session_access_tokens] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oauth_session_token_scopes] CHECK CONSTRAINT [FK_oauth_session_token_scopes_oauth_session_access_tokens]
GO
ALTER TABLE [dbo].[oauth_sessions]  WITH CHECK ADD  CONSTRAINT [FK_oauth_sessions_oauth_clients] FOREIGN KEY([client_id])
REFERENCES [dbo].[oauth_clients] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[oauth_sessions] CHECK CONSTRAINT [FK_oauth_sessions_oauth_clients]
GO
