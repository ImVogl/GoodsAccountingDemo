
CREATE TABLE public.goods_states
(
    index bigint NOT NULL,
    id uuid NOT NULL,
    write_off integer NOT NULL,
    receipt integer NOT NULL,
    storage integer NOT NULL,
    sold integer NOT NULL,
    "retailPrice" real NOT NULL,
    "wholeScalePrice" real NOT NULL,
    "WorkShiftIndex" bigint NOT NULL,
    CONSTRAINT "PK_goods_states" PRIMARY KEY (index),
    CONSTRAINT "FK_goods_states_work_shift_WorkShiftIndex" FOREIGN KEY ("WorkShiftIndex")
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

CREATE INDEX "IX_goods_states_WorkShiftIndex"
    ON public.goods_states USING btree
    ("WorkShiftIndex")
    TABLESPACE pg_default;

CREATE UNIQUE INDEX "IX_goods_states_index"
    ON public.goods_states USING btree
    (index)
    TABLESPACE pg_default;

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (1, 'd5e1e8c4-27fb-4cad-abfd-05a0833c5d1e', 0, 0, 10, 0, '205', '185', 1);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (2, '88f97004-06d9-4179-a8f7-53fa0d283aed', 0, 0, 10, 0, '105', '85', 1);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (3, '2ffd3c04-516e-4550-9b38-ec646a20b656', 0, 0, 10, 0, '105', '85', 1);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (4, '01363411-ac58-4c93-bc88-6391fe38915b', 0, 0, 10, 0, '100', '80', 1);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (5, '318564c0-6a92-45d2-b3e5-254aadd2cb0e', 0, 0, 10, 0, '205', '185', 1);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (6, '8e319673-0d2e-4b2c-bcbb-6690fe8aab6e', 0, 0, 10, 0, '205', '185', 1);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (7, '9a4afbd0-c0a9-442c-841c-70c88b6abf3e', 0, 0, 10, 0, '205', '185', 1);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (8, '3e9270a4-c9cb-4306-ace3-ba00a15fe675', 0, 0, 10, 0, '205', '185', 1);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (9, '9a0d8ea8-b2b0-4590-8138-cf1b3b3b3e35', 0, 0, 10, 0, '205', '185', 1);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (10, 'd5e1e8c4-27fb-4cad-abfd-05a0833c5d1e', 0, 0, 10, 0, '205', '185', 2);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (11, '88f97004-06d9-4179-a8f7-53fa0d283aed', 0, 0, 10, 0, '105', '85', 2);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (12, '2ffd3c04-516e-4550-9b38-ec646a20b656', 0, 0, 10, 0, '105', '85', 2);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (13, '01363411-ac58-4c93-bc88-6391fe38915b', 0, 0, 10, 0, '100', '80', 2);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (14, '318564c0-6a92-45d2-b3e5-254aadd2cb0e', 0, 0, 10, 0, '205', '185', 2);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (15, '8e319673-0d2e-4b2c-bcbb-6690fe8aab6e', 0, 0, 10, 0, '205', '185', 2);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (16, '9a4afbd0-c0a9-442c-841c-70c88b6abf3e', 0, 0, 10, 0, '205', '185', 2);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (17, '3e9270a4-c9cb-4306-ace3-ba00a15fe675', 0, 0, 10, 0, '205', '185', 2);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (18, '9a0d8ea8-b2b0-4590-8138-cf1b3b3b3e35', 0, 0, 10, 0, '205', '185', 2);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (19, 'd5e1e8c4-27fb-4cad-abfd-05a0833c5d1e', 0, 0, 10, 0, '205', '185', 3);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (20, '88f97004-06d9-4179-a8f7-53fa0d283aed', 0, 0, 10, 0, '105', '85', 3);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (21, '2ffd3c04-516e-4550-9b38-ec646a20b656', 0, 0, 10, 0, '105', '85', 3);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (22, '01363411-ac58-4c93-bc88-6391fe38915b', 0, 0, 10, 0, '100', '80', 3);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (23, '318564c0-6a92-45d2-b3e5-254aadd2cb0e', 0, 0, 10, 0, '205', '185', 3);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (24, '8e319673-0d2e-4b2c-bcbb-6690fe8aab6e', 0, 0, 10, 0, '205', '185', 3);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (25, '9a4afbd0-c0a9-442c-841c-70c88b6abf3e', 0, 0, 10, 0, '205', '185', 3);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (26, '3e9270a4-c9cb-4306-ace3-ba00a15fe675', 0, 0, 10, 0, '205', '185', 3);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (27, '9a0d8ea8-b2b0-4590-8138-cf1b3b3b3e35', 0, 0, 10, 0, '205', '185', 3);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (28, 'd5e1e8c4-27fb-4cad-abfd-05a0833c5d1e', 0, 0, 10, 0, '205', '185', 4);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (29, '88f97004-06d9-4179-a8f7-53fa0d283aed', 0, 0, 10, 0, '105', '85', 4);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (30, '2ffd3c04-516e-4550-9b38-ec646a20b656', 0, 0, 10, 0, '105', '85', 4);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (31, '01363411-ac58-4c93-bc88-6391fe38915b', 0, 0, 10, 0, '100', '80', 4);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (32, '318564c0-6a92-45d2-b3e5-254aadd2cb0e', 0, 0, 10, 0, '205', '185', 4);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (33, '8e319673-0d2e-4b2c-bcbb-6690fe8aab6e', 0, 0, 10, 0, '205', '185', 4);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (34, '9a4afbd0-c0a9-442c-841c-70c88b6abf3e', 0, 0, 10, 0, '205', '185', 4);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (35, '3e9270a4-c9cb-4306-ace3-ba00a15fe675', 0, 0, 10, 0, '205', '185', 4);

INSERT INTO public.goods_states(index, id, write_off, receipt, storage, sold, "retailPrice", "wholeScalePrice", "WorkShiftIndex")
	VALUES (36, '9a0d8ea8-b2b0-4590-8138-cf1b3b3b3e35', 0, 0, 10, 0, '205', '185', 4);
