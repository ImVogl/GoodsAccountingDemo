
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
	VALUES ('2b5be33e-ff8f-410e-81df-2dd0bcb41859', 'Греческий салат', 'Салаты', '80', '100', 10, true);

INSERT INTO public.goods(id, name, category, "wholeScalePrice", "retailPrice", store, active)
    VALUES ('34ebd79c-fc04-437d-b540-a4e1d0c43d67',	'Салат "Мимоза"',	'Салаты',	'85',	'105',	10,	false);

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
    VALUES ('f0f4776b-916f-463c-b8b4-ff9a3358d4e8',	'Винегрет',	'Салаты',	'85',	'105',	10,	true);
