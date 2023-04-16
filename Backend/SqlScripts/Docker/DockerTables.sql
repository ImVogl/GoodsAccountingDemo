-- Users section.
CREATE TABLE public.users
(
    id serial NOT NULL,
    login text COLLATE pg_catalog."default" NOT NULL,
    name text COLLATE pg_catalog."default" NOT NULL,
    surname text COLLATE pg_catalog."default" NOT NULL,
    role text COLLATE pg_catalog."default" NOT NULL,
    birth date NOT NULL,
    hash bytea NOT NULL,
    expired timestamp without time zone NOT NULL,
    salt text COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "PK_users" PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.users
    OWNER to postgres;

CREATE UNIQUE INDEX "IX_users_id"
    ON public.users USING btree
    (id)
    TABLESPACE pg_default;

INSERT INTO public.users(login, name, surname, role, birth, hash, expired, salt)
    VALUES ('user', 'Иван', 'Иванов', 'Administrator', '1990-06-15', decode('KuE6XqU08OIuLWs01YjxgM5VPwFYSHK/czhK3lU8O4U=', 'base64'), '2024-03-11 11:22:03.977218', '2f6f714e-89ea-4dd8-89fb-2e1f95e4c09a');

INSERT INTO public.users(login, name, surname, role, birth, hash, expired, salt)
    VALUES ('Petrov', 'Петр', 'Петров', 'RegisteredUser', '1995-07-19', decode('7UXPL5AFuVQbIgSckZ0i9JHhW28WKXQu6YLLZOH8Yws=', 'base64'), '2024-03-10 12:22:04.669666', '9023bf2e-e8dd-4008-8a7b-461da994a460');

-- Goods section
CREATE TABLE public.goods
(
    id uuid NOT NULL,
    name text COLLATE pg_catalog."default" NOT NULL,
    category text COLLATE pg_catalog."default" NOT NULL,
    "wholeScalePrice" real NOT NULL,
    "retailPrice" real NOT NULL,
    store integer NOT NULL,
    active boolean NOT NULL,
    CONSTRAINT "PK_goods" PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.goods
    OWNER to postgres;

CREATE UNIQUE INDEX "IX_goods_id"
    ON public.goods USING btree
    (id)
    TABLESPACE pg_default;

INSERT INTO public.goods(id, name, category, "wholeScalePrice", "retailPrice", store, active)
    VALUES ('3605a5de-9b31-42bd-83ff-ef3a6832c408',	'Ряженка',	'Напитки',	'185',	'205',	10,	true);

INSERT INTO public.goods(id, name, category, "wholeScalePrice", "retailPrice", store, active)
    VALUES ('6411a6fc-5b6a-4b2a-ae53-18fe968a7f02',	'Кофе',	'Напитки',	'185',	'205',	10,	true);

INSERT INTO public.goods(id, name, category, "wholeScalePrice", "retailPrice", store, active)
    VALUES ('6a5f301f-2aa3-48ca-92c1-4ef88458dc8a',	'Чай',	'Напитки',	'185',	'205',	10,	true);

INSERT INTO public.goods(id, name, category, "wholeScalePrice", "retailPrice", store, active)
    VALUES ('72b221e5-c1bf-42e3-9345-805b44ac69b7',	'Молоко',	'Напитки',	'185',	'205',	10,	true);

INSERT INTO public.goods(id, name, category, "wholeScalePrice", "retailPrice", store, active)
    VALUES ('a70e466b-ecc0-4fdd-99e4-d80c9c65b61d',	'Сок',	'Напитки',	'185',	'205',	10,	true);

INSERT INTO public.goods(id, name, category, "wholeScalePrice", "retailPrice", store, active)
    VALUES ('e6928f0f-be41-49fb-977e-e9d57af7a0b4',	'Кефир',	'Напитки',	'185',	'205',	10,	true);

INSERT INTO public.goods(id, name, category, "wholeScalePrice", "retailPrice", store, active)
	VALUES ('2b5be33e-ff8f-410e-81df-2dd0bcb41859', 'Греческий салат', 'Салаты', '80', '100', 10, true);

INSERT INTO public.goods(id, name, category, "wholeScalePrice", "retailPrice", store, active)
    VALUES ('34ebd79c-fc04-437d-b540-a4e1d0c43d67',	'Салат "Мимоза"',	'Салаты',	'85',	'105',	10,	false);

INSERT INTO public.goods(id, name, category, "wholeScalePrice", "retailPrice", store, active)
    VALUES ('f0f4776b-916f-463c-b8b4-ff9a3358d4e8',	'Винегрет',	'Салаты',	'85',	'105',	10,	true);

INSERT INTO public.goods(id, name, category, "wholeScalePrice", "retailPrice", store, active)
    VALUES ('7213d06b-0b42-41c7-b634-17f447845ac3',	'Салат "Цезарь"',	'Салаты',	'85',	'105',	10,	true);

INSERT INTO public.goods(id, name, category, "wholeScalePrice", "retailPrice", store, active)
    VALUES ('4421c4a1-d111-4eac-9e46-aeb53562b65e',	'Американо 0.2',	'Кофе',	'85',	'105',	10,	true);

INSERT INTO public.goods(id, name, category, "wholeScalePrice", "retailPrice", store, active)
    VALUES ('2de35b22-d326-4697-8462-a79d804e0c61',	'Американо 0.3',	'Кофе',	'85',	'105',	10,	true);

INSERT INTO public.goods(id, name, category, "wholeScalePrice", "retailPrice", store, active)
    VALUES ('8a18c7d6-aee6-48e8-9a07-bc8eb9d67b24',	'Американо 0.4',	'Кофе',	'85',	'105',	10,	true);

INSERT INTO public.goods(id, name, category, "wholeScalePrice", "retailPrice", store, active)
    VALUES ('62e0a9e9-f9f8-4a3f-bd39-ddb0ef2dff6d',	'Капучино 0.2',	'Салаты',	'85',	'105',	10,	true);

INSERT INTO public.goods(id, name, category, "wholeScalePrice", "retailPrice", store, active)
    VALUES ('94c48927-d0c6-4f4d-9474-325ab67e6a3c',	'Капучино 0.3',	'Салаты',	'85',	'105',	10,	true);

INSERT INTO public.goods(id, name, category, "wholeScalePrice", "retailPrice", store, active)
    VALUES ('fd5b6f1c-eb32-4946-8bd1-bd0aa555d4ef',	'Капучино 0.4',	'Салаты',	'85',	'105',	10,	true);

INSERT INTO public.goods(id, name, category, "wholeScalePrice", "retailPrice", store, active)
    VALUES ('2b08778e-920f-4f5f-92ed-c0d4c93772a9',	'Латте 0.2',	'Салаты',	'85',	'105',	10,	true);

INSERT INTO public.goods(id, name, category, "wholeScalePrice", "retailPrice", store, active)
    VALUES ('0d7c9ce6-3a7c-4f79-a3eb-3d7d82e60706',	'Латте 0.3',	'Салаты',	'85',	'105',	10,	true);

INSERT INTO public.goods(id, name, category, "wholeScalePrice", "retailPrice", store, active)
    VALUES ('157a1bf8-2627-446f-ad4d-73b90517fc4f',	'Латте 0.4',	'Салаты',	'85',	'105',	10,	true);

-- Working shift section
CREATE TABLE public.work_shift
(
    index serial NOT NULL,
    shift_open timestamp without time zone NOT NULL,
    shift_close timestamp without time zone NOT NULL,
    user_name text COLLATE pg_catalog."default" NOT NULL,
    user_id integer NOT NULL,
    cash integer NOT NULL,
    opened boolean NOT NULL,
    comments text COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "PK_work_shift" PRIMARY KEY (index)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.work_shift
    OWNER to postgres;

CREATE UNIQUE INDEX "IX_work_shift_index"
    ON public.work_shift USING btree
    (index)
    TABLESPACE pg_default;

-- Goods states section
CREATE TABLE public.goods_states
(
    index serial NOT NULL,
    id uuid NOT NULL,
    write_off integer NOT NULL,
    receipt integer NOT NULL,
    storage integer NOT NULL,
    sold integer NOT NULL,
    retail_price real NOT NULL,
    whole_scale_price real NOT NULL,
    shift_identifier bigint NOT NULL,
    CONSTRAINT "PK_goods_states" PRIMARY KEY (index),
    CONSTRAINT "FK_goods_states_work_shift_shift_identifier" FOREIGN KEY (shift_identifier)
        REFERENCES public.work_shift (index) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.goods_states
    OWNER to postgres;

CREATE UNIQUE INDEX "IX_goods_states_index"
    ON public.goods_states USING btree
    (index)
    TABLESPACE pg_default;

CREATE INDEX "IX_goods_states_shift_identifier"
    ON public.goods_states USING btree
    (shift_identifier)
    TABLESPACE pg_default;
