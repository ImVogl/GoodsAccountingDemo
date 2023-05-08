
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
