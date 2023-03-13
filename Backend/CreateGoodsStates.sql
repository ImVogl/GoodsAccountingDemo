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

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('2b5be33e-ff8f-410e-81df-2dd0bcb41859', 0, 0, 10, 0, '205', '185', 1);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('34ebd79c-fc04-437d-b540-a4e1d0c43d67', 0, 0, 10, 0, '105', '85', 1);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('2ffd3c04-516e-4550-9b38-ec646a20b656', 0, 0, 10, 0, '105', '85', 1);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('6411a6fc-5b6a-4b2a-ae53-18fe968a7f02', 0, 0, 10, 0, '100', '80', 1);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('6a5f301f-2aa3-48ca-92c1-4ef88458dc8a', 0, 0, 10, 0, '205', '185', 1);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('72b221e5-c1bf-42e3-9345-805b44ac69b7', 0, 0, 10, 0, '205', '185', 1);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('a70e466b-ecc0-4fdd-99e4-d80c9c65b61d', 0, 0, 10, 0, '205', '185', 1);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('e6928f0f-be41-49fb-977e-e9d57af7a0b4', 0, 0, 10, 0, '205', '185', 1);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('f0f4776b-916f-463c-b8b4-ff9a3358d4e8', 0, 0, 10, 0, '205', '185', 1);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('2b5be33e-ff8f-410e-81df-2dd0bcb41859', 0, 0, 10, 0, '205', '185', 2);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('34ebd79c-fc04-437d-b540-a4e1d0c43d67', 0, 0, 10, 0, '105', '85', 2);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('2ffd3c04-516e-4550-9b38-ec646a20b656', 0, 0, 10, 0, '105', '85', 2);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('6411a6fc-5b6a-4b2a-ae53-18fe968a7f02', 0, 0, 10, 0, '100', '80', 2);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('6a5f301f-2aa3-48ca-92c1-4ef88458dc8a', 0, 0, 10, 0, '205', '185', 2);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('72b221e5-c1bf-42e3-9345-805b44ac69b7', 0, 0, 10, 0, '205', '185', 2);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('a70e466b-ecc0-4fdd-99e4-d80c9c65b61d', 0, 0, 10, 0, '205', '185', 2);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('e6928f0f-be41-49fb-977e-e9d57af7a0b4', 0, 0, 10, 0, '205', '185', 2);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('f0f4776b-916f-463c-b8b4-ff9a3358d4e8', 0, 0, 10, 0, '205', '185', 2);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('2b5be33e-ff8f-410e-81df-2dd0bcb41859', 0, 0, 10, 0, '205', '185', 3);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('34ebd79c-fc04-437d-b540-a4e1d0c43d67', 0, 0, 10, 0, '105', '85', 3);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('2ffd3c04-516e-4550-9b38-ec646a20b656', 0, 0, 10, 0, '105', '85', 3);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('6411a6fc-5b6a-4b2a-ae53-18fe968a7f02', 0, 0, 10, 0, '100', '80', 3);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('6a5f301f-2aa3-48ca-92c1-4ef88458dc8a', 0, 0, 10, 0, '205', '185', 3);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('72b221e5-c1bf-42e3-9345-805b44ac69b7', 0, 0, 10, 0, '205', '185', 3);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('a70e466b-ecc0-4fdd-99e4-d80c9c65b61d', 0, 0, 10, 0, '205', '185', 3);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('e6928f0f-be41-49fb-977e-e9d57af7a0b4', 0, 0, 10, 0, '205', '185', 3);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('f0f4776b-916f-463c-b8b4-ff9a3358d4e8', 0, 0, 10, 0, '205', '185', 3);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('2b5be33e-ff8f-410e-81df-2dd0bcb41859', 0, 0, 10, 0, '205', '185', 4);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('34ebd79c-fc04-437d-b540-a4e1d0c43d67', 0, 0, 10, 0, '105', '85', 4);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('2ffd3c04-516e-4550-9b38-ec646a20b656', 0, 0, 10, 0, '105', '85', 4);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('6411a6fc-5b6a-4b2a-ae53-18fe968a7f02', 0, 0, 10, 0, '100', '80', 4);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('6a5f301f-2aa3-48ca-92c1-4ef88458dc8a', 0, 0, 10, 0, '205', '185', 4);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('72b221e5-c1bf-42e3-9345-805b44ac69b7', 0, 0, 10, 0, '205', '185', 4);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('a70e466b-ecc0-4fdd-99e4-d80c9c65b61d', 0, 0, 10, 0, '205', '185', 4);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('e6928f0f-be41-49fb-977e-e9d57af7a0b4', 0, 0, 10, 0, '205', '185', 4);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('f0f4776b-916f-463c-b8b4-ff9a3358d4e8', 0, 0, 10, 0, '205', '185', 4);
