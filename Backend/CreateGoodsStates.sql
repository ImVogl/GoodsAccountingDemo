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
	VALUES ('d5e1e8c4-27fb-4cad-abfd-05a0833c5d1e', 0, 0, 10, 0, '205', '185', 1);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('88f97004-06d9-4179-a8f7-53fa0d283aed', 0, 0, 10, 0, '105', '85', 1);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('2ffd3c04-516e-4550-9b38-ec646a20b656', 0, 0, 10, 0, '105', '85', 1);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('01363411-ac58-4c93-bc88-6391fe38915b', 0, 0, 10, 0, '100', '80', 1);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('318564c0-6a92-45d2-b3e5-254aadd2cb0e', 0, 0, 10, 0, '205', '185', 1);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('8e319673-0d2e-4b2c-bcbb-6690fe8aab6e', 0, 0, 10, 0, '205', '185', 1);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('9a4afbd0-c0a9-442c-841c-70c88b6abf3e', 0, 0, 10, 0, '205', '185', 1);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('3e9270a4-c9cb-4306-ace3-ba00a15fe675', 0, 0, 10, 0, '205', '185', 1);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('9a0d8ea8-b2b0-4590-8138-cf1b3b3b3e35', 0, 0, 10, 0, '205', '185', 1);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('d5e1e8c4-27fb-4cad-abfd-05a0833c5d1e', 0, 0, 10, 0, '205', '185', 2);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('88f97004-06d9-4179-a8f7-53fa0d283aed', 0, 0, 10, 0, '105', '85', 2);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('2ffd3c04-516e-4550-9b38-ec646a20b656', 0, 0, 10, 0, '105', '85', 2);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('01363411-ac58-4c93-bc88-6391fe38915b', 0, 0, 10, 0, '100', '80', 2);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('318564c0-6a92-45d2-b3e5-254aadd2cb0e', 0, 0, 10, 0, '205', '185', 2);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('8e319673-0d2e-4b2c-bcbb-6690fe8aab6e', 0, 0, 10, 0, '205', '185', 2);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('9a4afbd0-c0a9-442c-841c-70c88b6abf3e', 0, 0, 10, 0, '205', '185', 2);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('3e9270a4-c9cb-4306-ace3-ba00a15fe675', 0, 0, 10, 0, '205', '185', 2);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('9a0d8ea8-b2b0-4590-8138-cf1b3b3b3e35', 0, 0, 10, 0, '205', '185', 2);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('d5e1e8c4-27fb-4cad-abfd-05a0833c5d1e', 0, 0, 10, 0, '205', '185', 3);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('88f97004-06d9-4179-a8f7-53fa0d283aed', 0, 0, 10, 0, '105', '85', 3);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('2ffd3c04-516e-4550-9b38-ec646a20b656', 0, 0, 10, 0, '105', '85', 3);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('01363411-ac58-4c93-bc88-6391fe38915b', 0, 0, 10, 0, '100', '80', 3);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('318564c0-6a92-45d2-b3e5-254aadd2cb0e', 0, 0, 10, 0, '205', '185', 3);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('8e319673-0d2e-4b2c-bcbb-6690fe8aab6e', 0, 0, 10, 0, '205', '185', 3);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('9a4afbd0-c0a9-442c-841c-70c88b6abf3e', 0, 0, 10, 0, '205', '185', 3);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('3e9270a4-c9cb-4306-ace3-ba00a15fe675', 0, 0, 10, 0, '205', '185', 3);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('9a0d8ea8-b2b0-4590-8138-cf1b3b3b3e35', 0, 0, 10, 0, '205', '185', 3);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('d5e1e8c4-27fb-4cad-abfd-05a0833c5d1e', 0, 0, 10, 0, '205', '185', 4);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('88f97004-06d9-4179-a8f7-53fa0d283aed', 0, 0, 10, 0, '105', '85', 4);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('2ffd3c04-516e-4550-9b38-ec646a20b656', 0, 0, 10, 0, '105', '85', 4);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('01363411-ac58-4c93-bc88-6391fe38915b', 0, 0, 10, 0, '100', '80', 4);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('318564c0-6a92-45d2-b3e5-254aadd2cb0e', 0, 0, 10, 0, '205', '185', 4);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('8e319673-0d2e-4b2c-bcbb-6690fe8aab6e', 0, 0, 10, 0, '205', '185', 4);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('9a4afbd0-c0a9-442c-841c-70c88b6abf3e', 0, 0, 10, 0, '205', '185', 4);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('3e9270a4-c9cb-4306-ace3-ba00a15fe675', 0, 0, 10, 0, '205', '185', 4);

INSERT INTO public.goods_states(id, write_off, receipt, storage, sold, retail_price, whole_scale_price, shift_identifier)
	VALUES ('9a0d8ea8-b2b0-4590-8138-cf1b3b3b3e35', 0, 0, 10, 0, '205', '185', 4);
